using Microsoft.EntityFrameworkCore;

namespace MotoMobiNardo.Data;

public sealed class PostgresMotoMobiDbContext(DbContextOptions<PostgresMotoMobiDbContext> options)
    : MotoMobiDbContext(options);
