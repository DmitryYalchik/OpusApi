using Microsoft.EntityFrameworkCore;
using OpusApi.DbModels;

namespace OpusApi;

public sealed class SqliteDbContext : DbContext
{
    public DbSet<SearcherEntity> Searchers => Set<SearcherEntity>();
    public DbSet<GroupEntity> Groups => Set<GroupEntity>();
    public DbSet<IncidentEntity> Incidents => Set<IncidentEntity>();

    // Используется приложением: строка подключения задаётся в OnConfiguring.
    public SqliteDbContext() => Database.EnsureCreated();

    // Используется, когда конфигурация передаётся извне (например, тесты с in-memory SQLite).
    public SqliteDbContext(DbContextOptions<SqliteDbContext> options) : base(options)
        => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=database.sqlite");
        }
    }
}
