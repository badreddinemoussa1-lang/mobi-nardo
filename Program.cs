using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MotoMobiNardo.Data;
using Npgsql;
using System.Globalization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("acquisition-inquiries", context =>
    {
        var remoteIp = context.Connection.RemoteIpAddress;
        var cloudflareIp = context.Request.Headers["CF-Connecting-IP"].ToString();
        var partitionKey = remoteIp is not null && System.Net.IPAddress.IsLoopback(remoteIp) &&
                           System.Net.IPAddress.TryParse(cloudflareIp, out var visitorIp)
            ? visitorIp.ToString()
            : remoteIp?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.AddPolicy("customer-inquiries", context =>
    {
        var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            remoteIp,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});
var demoMode = string.Equals(builder.Configuration["MOBI_NARDO_DEMO_MODE"], "true", StringComparison.OrdinalIgnoreCase);
var postgresConnection = builder.Configuration["DATABASE_URL"]
    ?? builder.Configuration["POSTGRES_URL"]
    ?? builder.Configuration.GetConnectionString("Postgres");
var runningOnVercel = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("VERCEL"));
if (runningOnVercel && string.IsNullOrWhiteSpace(postgresConnection))
    throw new InvalidOperationException("Configure a durable PostgreSQL DATABASE_URL integration before deploying this app to Vercel.");

if (!string.IsNullOrWhiteSpace(postgresConnection))
{
    var connectionString = NormalizePostgresConnectionString(postgresConnection);
    builder.Services.AddDbContext<PostgresMotoMobiDbContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddScoped<MotoMobiDbContext>(services => services.GetRequiredService<PostgresMotoMobiDbContext>());
    builder.Services.AddDataProtection().PersistKeysToDbContext<PostgresMotoMobiDbContext>();
}
else
{
    var configuredDatabaseDirectory = Environment.GetEnvironmentVariable("MOBI_NARDO_DATA_PATH");
    var databaseDirectory = string.IsNullOrWhiteSpace(configuredDatabaseDirectory)
        ? Path.Combine(builder.Environment.ContentRootPath, "App_Data")
        : Path.GetFullPath(configuredDatabaseDirectory);
    if (demoMode && !string.Equals(Path.GetFileName(databaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), "mobi-nardo-demo", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Demo mode requires a separate data directory named 'mobi-nardo-demo'. Use tools/Start-Demo.ps1 so demo records cannot mix with live data.");
    Directory.CreateDirectory(databaseDirectory);
    var databasePath = Path.Combine(databaseDirectory, "moto-mobi-nardo.db");
    builder.Services.AddDbContext<MotoMobiDbContext>(options =>
        options.UseSqlite($"Data Source={databasePath};Foreign Keys=True;"));
}
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 12;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
})
    .AddEntityFrameworkStores<MotoMobiNardo.Data.MotoMobiDbContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = appEnvironmentIsDevelopment(builder.Environment)
        ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<MotoMobiNardo.Data.MotoMobiDbContext>();
    database.Database.Migrate();
    if (!database.BusinessProfiles.Any())
    {
        var profile = builder.Configuration.GetSection("BusinessProfile").Get<MotoMobiNardo.Models.BusinessProfile>() ?? new MotoMobiNardo.Models.BusinessProfile();
        profile.Id = 1;
        if (demoMode && !profile.Name.Contains("DÉMO", StringComparison.OrdinalIgnoreCase)) profile.Name += " · DÉMO";
        database.BusinessProfiles.Add(profile);
        database.SaveChanges();
    }
    if (!database.Motorcycles.Any())
    {
        var publicInventory = builder.Configuration.GetSection("InitialMotorcycles").Get<List<MotoMobiNardo.Models.Motorcycle>>() ?? [];
        if (demoMode)
            publicInventory.AddRange(CreateDemoMotorcycles());
        if (publicInventory.Count > 0)
        {
            database.Motorcycles.AddRange(publicInventory);
            database.SaveChanges();
        }
        if (demoMode)
            SeedDemoBusinessData(database);
    }
}

// Configure the HTTP request pipeline.
var forwardedHeaders = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeaders.KnownIPNetworks.Clear();
forwardedHeaders.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeaders);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.MapGet("/health", () => Results.Json(new { status = "ok" })).AllowAnonymous();

app.UseStaticFiles();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("fr-MA")
    .AddSupportedCultures("fr-MA")
    .AddSupportedUICultures("fr-MA"));
app.UseRouting();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Showroom}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

static bool appEnvironmentIsDevelopment(IWebHostEnvironment environment) => environment.IsDevelopment();

static string NormalizePostgresConnectionString(string value)
{
    if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
        (uri.Scheme != "postgres" && uri.Scheme != "postgresql"))
        return value;

    var separator = uri.UserInfo.IndexOf(':');
    var username = separator < 0 ? uri.UserInfo : uri.UserInfo[..separator];
    var password = separator < 0 ? "" : uri.UserInfo[(separator + 1)..];
    var settings = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.IsDefaultPort ? 5432 : uri.Port,
        Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
        Username = Uri.UnescapeDataString(username),
        Password = Uri.UnescapeDataString(password),
        SslMode = SslMode.Require,
        Timeout = 15,
        CommandTimeout = 30,
        MaxPoolSize = 5
    };
    return settings.ConnectionString;
}

static List<MotoMobiNardo.Models.Motorcycle> CreateDemoMotorcycles() =>
[
    new() { Brand = "HONDA", Model = "CB125F", Year = DateTime.Today.Year - 1, EngineCc = 125, Price = 14500, Stock = 3, Category = MotoMobiNardo.Models.MotorcycleCategories.Motorcycle, Condition = MotoMobiNardo.Models.MotorcycleConditions.Used, Description = "DÉMONSTRATION — exemple de fiche; confirmer prix, état et disponibilité." },
    new() { Brand = "YAMAHA", Model = "NMAX 155", Year = DateTime.Today.Year - 2, EngineCc = 155, Price = 32000, Stock = 2, Category = MotoMobiNardo.Models.MotorcycleCategories.Scooter, Condition = MotoMobiNardo.Models.MotorcycleConditions.Used, Description = "DÉMONSTRATION — exemple de scooter; données fictives." },
    new() { Brand = "KYMCO", Model = "AGILITY 125", Year = DateTime.Today.Year - 1, EngineCc = 125, Price = 20000, Stock = 3, Category = MotoMobiNardo.Models.MotorcycleCategories.Scooter, Condition = MotoMobiNardo.Models.MotorcycleConditions.New, Description = "DÉMONSTRATION — exemple de fiche; données fictives." },
    new() { Brand = "SUZUKI", Model = "ADDRESS 110", Year = DateTime.Today.Year - 3, EngineCc = 110, Price = 13500, Stock = 1, Category = MotoMobiNardo.Models.MotorcycleCategories.Scooter, Condition = MotoMobiNardo.Models.MotorcycleConditions.Used, Description = "DÉMONSTRATION — exemple de fiche; données fictives." }
];

static void SeedDemoBusinessData(MotoMobiNardo.Data.MotoMobiDbContext database)
{
    var motorcycles = database.Motorcycles.ToList();
    var customers = new List<MotoMobiNardo.Models.Customer>
    {
        new() { FullName = "DÉMO · Salma Exemple", Phone = "+212600000101", Email = "salma@example.test", Address = "Bouskoura · donnée fictive" },
        new() { FullName = "DÉMO · Yassine Exemple", Phone = "+212600000102", Email = "yassine@example.test", Address = "Casablanca · donnée fictive" },
        new() { FullName = "DÉMO · Lina Exemple", Phone = "+212600000103", Email = "lina@example.test", Address = "Bouskoura · donnée fictive" }
    };
    database.Customers.AddRange(customers);
    database.SaveChanges();

    var demoSales = new[]
    {
        (Model: "HONDA CB125F", Customer: customers[0], DaysAgo: 72, Channel: MotoMobiNardo.Models.SalesChannels.Showroom),
        (Model: "KYMCO AGILITY 125", Customer: customers[1], DaysAgo: 38, Channel: MotoMobiNardo.Models.SalesChannels.WhatsApp),
        (Model: "YAMAHA NMAX 155", Customer: customers[2], DaysAgo: 11, Channel: MotoMobiNardo.Models.SalesChannels.Website)
    };
    foreach (var item in demoSales)
    {
        var motorcycle = motorcycles.Single(m => m.FullName == item.Model);
        motorcycle.Stock--;
        database.Sales.Add(new MotoMobiNardo.Models.Sale
        {
            MotorcycleId = motorcycle.Id,
            CustomerId = item.Customer.Id,
            Channel = item.Channel,
            Quantity = 1,
            UnitPrice = motorcycle.Price,
            SoldAt = DateTime.Today.AddDays(-item.DaysAgo)
        });
    }

    database.CustomerInquiries.AddRange(
        new MotoMobiNardo.Models.CustomerInquiry { Name = "DÉMO · Amine Prospect", Phone = "+212600000111", Email = "amine@example.test", Type = MotoMobiNardo.Models.CustomerInquiryTypes.Availability, MotorcycleId = motorcycles.Single(m => m.FullName == "HONDA CB125F").Id, Message = "Donnée fictive pour montrer une demande de disponibilité.", Status = MotoMobiNardo.Models.CustomerInquiryStatuses.New },
        new MotoMobiNardo.Models.CustomerInquiry { Name = "DÉMO · Hajar Prospect", Phone = "+212600000112", Email = "hajar@example.test", Type = MotoMobiNardo.Models.CustomerInquiryTypes.Visit, MotorcycleId = motorcycles.Single(m => m.FullName == "KYMCO AGILITY 125").Id, Message = "Donnée fictive pour montrer l’étape de suivi.", Status = MotoMobiNardo.Models.CustomerInquiryStatuses.Contacted },
        new MotoMobiNardo.Models.CustomerInquiry { Name = "DÉMO · Karim Prospect", Phone = "+212600000113", Email = "karim@example.test", Type = MotoMobiNardo.Models.CustomerInquiryTypes.Reservation, MotorcycleId = motorcycles.Single(m => m.FullName == "SUZUKI ADDRESS 110").Id, Message = "Demande fictive; aucune réservation réelle n’est créée.", Status = MotoMobiNardo.Models.CustomerInquiryStatuses.Reserved }
    );
    database.SaveChanges();
}
