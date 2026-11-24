using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MotoMobiNardo.Models;

namespace MotoMobiNardo.Data;

public class MotoMobiDbContext(DbContextOptions<MotoMobiDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<AcquisitionInquiry> AcquisitionInquiries => Set<AcquisitionInquiry>();
    public DbSet<CustomerInquiry> CustomerInquiries => Set<CustomerInquiry>();
    public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Motorcycle>().Property(m => m.Stock).IsConcurrencyToken();
        modelBuilder.Entity<Motorcycle>().HasIndex(m => new { m.Brand, m.Model });
        modelBuilder.Entity<Customer>().HasIndex(c => c.Phone);
        modelBuilder.Entity<Sale>().Property(s => s.UnitPrice).HasPrecision(12, 2);
        modelBuilder.Entity<Sale>().Property(s => s.Discount).HasPrecision(12, 2);
        modelBuilder.Entity<Sale>().Property(s => s.Channel).HasMaxLength(32).HasDefaultValue(SalesChannels.Unspecified);
        modelBuilder.Entity<Sale>().HasOne<Motorcycle>().WithMany().HasForeignKey(s => s.MotorcycleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Sale>().HasOne<Customer>().WithMany().HasForeignKey(s => s.CustomerId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AcquisitionInquiry>().HasIndex(i => i.CreatedAtUtc);
        modelBuilder.Entity<CustomerInquiry>().HasIndex(i => new { i.Status, i.CreatedAtUtc });
        modelBuilder.Entity<CustomerInquiry>().HasIndex(i => i.Phone);
        modelBuilder.Entity<CustomerInquiry>().HasOne<Motorcycle>().WithMany().HasForeignKey(i => i.MotorcycleId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<BusinessProfile>().HasIndex(p => p.Id);
    }
}
