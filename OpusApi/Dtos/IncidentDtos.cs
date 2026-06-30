namespace OpusApi.Dtos;

/// <summary>
/// Данные для создания или обновления записи журнала связи.
/// </summary>
public class IncidentRequest
{
    /// <summary>Отправитель сообщения. Обязательное поле, максимум 50 символов.</summary>
    /// <example>Штаб</example>
    public string From { get; set; } = null!;

    /// <summary>Получатель сообщения. Обязательное поле, максимум 50 символов.</summary>
    /// <example>Группа №1</example>
    public string To { get; set; } = null!;

    /// <summary>Текст сообщения. Обязательное поле, максимум 350 символов.</summary>
    /// <example>Выдвигайтесь на квадрат 14, обследуйте русло реки.</example>
    public string Message { get; set; } = null!;
}

/// <summary>
/// Запись журнала связи — зафиксированное сообщение между участниками поиска.
/// </summary>
public class IncidentResponse
{
    /// <summary>Уникальный идентификатор записи.</summary>
    public Guid Id { get; set; }

    /// <summary>Дата и время создания записи.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Отправитель сообщения.</summary>
    /// <example>Штаб</example>
    public string From { get; set; } = null!;

    /// <summary>Получатель сообщения.</summary>
    /// <example>Группа №1</example>
    public string To { get; set; } = null!;

    /// <summary>Текст сообщения.</summary>
    /// <example>Выдвигайтесь на квадрат 14, обследуйте русло реки.</example>
    public string Message { get; set; } = null!;
}
