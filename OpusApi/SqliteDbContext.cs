using Microsoft.EntityFrameworkCore;
using OpusApi.DbModels;

namespace OpusApi;

public sealed class SqliteDbContext : DbContext
{
    public DbSet<SearcherEntity> Searchers => Set<SearcherEntity>();
    public DbSet<GroupEntity> Groups => Set<GroupEntity>();
    
    public SqliteDbContext() => Database.EnsureCreated();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.sqlite");
    }
}