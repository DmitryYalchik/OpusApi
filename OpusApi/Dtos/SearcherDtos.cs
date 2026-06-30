namespace OpusApi.Dtos;

/// <summary>
/// Данные для создания или обновления поисковика.
/// </summary>
public class SearcherRequest
{
    /// <summary>Фамилия. Обязательное поле, от 3 до 50 символов.</summary>
    /// <example>Иванов</example>
    public string LastName { get; set; } = null!;

    /// <summary>Имя. Обязательное поле, от 3 до 50 символов.</summary>
    /// <example>Пётр</example>
    public string FirstName { get; set; } = null!;

    /// <summary>Отчество. Необязательное поле; при наличии — от 3 до 50 символов.</summary>
    /// <example>Сергеевич</example>
    public string? FatherName { get; set; }

    /// <summary>Позывной поисковика. Необязательное поле.</summary>
    /// <example>Сокол</example>
    public string? NickName { get; set; }

    /// <summary>Контактный номер телефона. Обязательное поле.</summary>
    /// <example>+79991234567</example>
    public string PhoneNumber { get; set; } = null!;
}

/// <summary>
/// Поисковик — участник поисково-спасательных работ.
/// </summary>
public class SearcherResponse
{
    /// <summary>Уникальный идентификатор поисковика.</summary>
    public Guid Id { get; set; }

    /// <summary>Дата и время создания записи.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Фамилия.</summary>
    /// <example>Иванов</example>
    public string LastName { get; set; } = null!;

    /// <summary>Имя.</summary>
    /// <example>Пётр</example>
    public string FirstName { get; set; } = null!;

    /// <summary>Отчество.</summary>
    /// <example>Сергеевич</example>
    public string? FatherName { get; set; }

    /// <summary>Позывной поисковика.</summary>
    /// <example>Сокол</example>
    public string? NickName { get; set; }

    /// <summary>Контактный номер телефона.</summary>
    /// <example>+79991234567</example>
    public string PhoneNumber { get; set; } = null!;
}
