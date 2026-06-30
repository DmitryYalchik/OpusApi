using System.Threading.Tasks;
using OpusApi.DbModels;
using OpusApi.Dtos;

namespace OpusApi.Tests.Environments;

/// <summary>
/// Окружение для тестов поисковиков: наполняет БД данными и готовит запросы к контроллеру.
/// </summary>
public class SearcherEnvironment(SqliteDbContext db)
{
    /// <summary>Добавляет поисковика в БД и возвращает сохранённую сущность.</summary>
    public async Task<SearcherEntity> AddSearcherAsync(string lastName = "Иванов")
    {
        var entity = new SearcherEntity
        {
            LastName = lastName,
            FirstName = "Пётр",
            PhoneNumber = "+79991234567"
        };

        db.Searchers.Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }

    /// <summary>Запрос на создание/обновление поисковика (по умолчанию валидный).</summary>
    public SearcherRequest Request(string lastName = "Иванов", string firstName = "Пётр") => new()
    {
        LastName = lastName,
        FirstName = firstName,
        PhoneNumber = "+79991234567"
    };
}
