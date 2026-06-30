using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpusApi;

namespace OpusApi.Tests;

/// <summary>
/// База для тестов на реальной БД. Поднимает SQLite в режиме :memory: — тот же движок
/// и та же схема, что в проде, но всё живёт в памяти и изолировано на каждый тест.
/// </summary>
public abstract class SqliteTestBase
{
    private SqliteConnection _connection = null!;

    /// <summary>Контекст «под рукой» для подготовки данных и проверок.</summary>
    protected SqliteDbContext DbContext { get; private set; } = null!;

    [TestInitialize]
    public void InitDatabase()
    {
        // Открытое соединение держит in-memory БД живой на всё время теста;
        // как только оно закроется — база исчезнет.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        DbContext = CreateContext();
    }

    [TestCleanup]
    public void DisposeDatabase()
    {
        DbContext.Dispose();
        _connection.Dispose();
    }

    /// <summary>
    /// Свежий контекст на том же соединении (с чистым change-tracker'ом).
    /// Нужен, чтобы проверять реально сохранённые в БД данные, а не закэшированные в памяти.
    /// </summary>
    protected SqliteDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SqliteDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new SqliteDbContext(options);
    }
}
