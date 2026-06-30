using System.Collections.Generic;
using System.Threading.Tasks;
using OpusApi.DbModels;

namespace OpusApi.Tests.Environments;

/// <summary>
/// Окружение для тестов подключений: наполняет in-memory БД и возвращает сохранённые сущности.
/// </summary>
public class ConnectionEnvironment(InMemoryDbContext db)
{
    /// <summary>Добавляет подключение в БД и возвращает сохранённую сущность.</summary>
    public async Task<ConnectionEntity> AddConnectionAsync(
        string userName = "Оператор Иванов",
        string connectionId = "conn-1")
    {
        var entity = new ConnectionEntity
        {
            UserName = userName,
            ConnectionId = new List<string> { connectionId }
        };

        db.Connections.Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }
}
