using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MotoMobiNardo.Data;

public class MotoMobiDbContextFactory : IDesignTimeDbContextFactory<MotoMobiDbContext>
{
    public MotoMobiDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MotoMobiDbContext>()
            .UseSqlite("Data Source=App_Data/moto-mobi-nardo.db;Foreign Keys=True;")
            .Options;
        return new MotoMobiDbContext(options);
    }
}
