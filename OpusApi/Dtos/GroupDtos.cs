namespace OpusApi.Dtos;

/// <summary>
/// Данные для создания или обновления группы.
/// Отвечает только за саму группу — поисковики этим запросом не создаются.
/// </summary>
public class GroupRequest
{
    /// <summary>Название группы. Обязательное поле, максимум 50 символов.</summary>
    /// <example>Группа №1</example>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Группа — объединение поисковиков на поиске.
/// </summary>
public class GroupResponse
{
    /// <summary>Уникальный идентификатор группы.</summary>
    public Guid Id { get; set; }

    /// <summary>Дата и время создания записи.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Название группы.</summary>
    /// <example>Группа №1</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>Поисковики, входящие в группу (только для чтения).</summary>
    public IEnumerable<SearcherResponse> Searchers { get; set; } = [];
}
