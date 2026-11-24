using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MotoMobiNardo.Data;

public sealed class PostgresMotoMobiDbContextFactory : IDesignTimeDbContextFactory<PostgresMotoMobiDbContext>
{
    public PostgresMotoMobiDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PostgresMotoMobiDbContext>()
            .UseNpgsql("Host=localhost;Database=mobi_nardo;Username=postgres;Password=local-design-time-only")
            .Options;
        return new PostgresMotoMobiDbContext(options);
    }
}
