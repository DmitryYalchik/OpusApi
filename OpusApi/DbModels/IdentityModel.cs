using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpusApi.DbModels;

/// <summary>
/// Базовая модель для всех сущностей: уникальный идентификатор, дата создания
/// и контракт валидации.
/// </summary>
public abstract class IdentityModel
{
    /// <summary>
    /// Уникальный идентификатор сущности (GUID версии 7 — сортируемый по времени).
    /// Присваивается автоматически при создании.
    /// </summary>
    [Column("id")] [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// Дата и время создания записи. Присваивается автоматически.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Проверяет корректность данных сущности.
    /// </summary>
    /// <param name="errorMessage">
    /// Сообщение с описанием ошибок валидации; <c>null</c> или пустая строка, если ошибок нет.
    /// </param>
    /// <returns><c>true</c>, если данные корректны; иначе <c>false</c>.</returns>
    public abstract bool Validate(out string? errorMessage);
}
