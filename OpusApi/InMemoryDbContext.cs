using Microsoft.EntityFrameworkCore;
using OpusApi.DbModels;

namespace OpusApi;

public sealed class InMemoryDbContext : DbContext
{
    public DbSet<ConnectionEntity> Connections => Set<ConnectionEntity>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(databaseName: "database");
    }
}