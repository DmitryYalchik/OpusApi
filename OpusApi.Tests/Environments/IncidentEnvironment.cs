using System.Threading.Tasks;
using OpusApi.DbModels;
using OpusApi.Dtos;

namespace OpusApi.Tests.Environments;

/// <summary>
/// Окружение для тестов журнала связи: наполняет БД данными и готовит запросы к контроллеру.
/// </summary>
public class IncidentEnvironment(SqliteDbContext db)
{
    /// <summary>Добавляет запись журнала в БД и возвращает сохранённую сущность.</summary>
    public async Task<IncidentEntity> AddIncidentAsync(string message = "Выдвигайтесь на квадрат 14.")
    {
        var entity = new IncidentEntity
        {
            From = "Штаб",
            To = "Группа №1",
            Message = message
        };

        db.Incidents.Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }

    /// <summary>Запрос на создание/обновление записи журнала (по умолчанию валидный).</summary>
    public IncidentRequest Request() => new()
    {
        From = "Штаб",
        To = "Группа №1",
        Message = "Выдвигайтесь на квадрат 14."
    };
}
