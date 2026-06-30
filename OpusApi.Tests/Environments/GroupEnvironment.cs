using System.Collections.Generic;
using System.Threading.Tasks;
using OpusApi.DbModels;
using OpusApi.Dtos;

namespace OpusApi.Tests.Environments;

/// <summary>
/// Окружение для тестов групп: наполняет БД данными и готовит запросы к контроллеру.
/// </summary>
public class GroupEnvironment(SqliteDbContext db)
{
    /// <summary>Запрос на создание/обновление группы (по умолчанию валидный).</summary>
    public GroupRequest Request(string name = "Группа №1") => new() { Name = name };

    /// <summary>Добавляет группу в БД и возвращает сохранённую сущность.</summary>
    public async Task<GroupEntity> AddGroupAsync(string name = "Группа №1")
    {
        var entity = new GroupEntity { Name = name };

        db.Groups.Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }

    /// <summary>Добавляет группу с одним связанным поисковиком и возвращает сохранённую группу.</summary>
    public async Task<GroupEntity> AddGroupWithSearcherAsync(
        string name = "Группа №1",
        string searcherLastName = "Иванов")
    {
        var entity = new GroupEntity
        {
            Name = name,
            Searchers = new List<SearcherEntity>
            {
                new() { LastName = searcherLastName, FirstName = "Пётр", PhoneNumber = "+79991234567" }
            }
        };

        db.Groups.Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }
}
