using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OpusApi.DbModels;

/// <summary>
/// Поисковик — участник поисково-спасательных работ.
/// </summary>
[Table("searcher_entity")]
public class SearcherEntity : IdentityModel
{
    /// <summary>Фамилия. Обязательное поле, от 3 до 50 символов.</summary>
    /// <example>Иванов</example>
    [Column("last_name")]
    public string LastName { get; set; } = null!;

    /// <summary>Имя. Обязательное поле, от 3 до 50 символов.</summary>
    /// <example>Пётр</example>
    [Column("first_name")]
    public string FirstName { get; set; } = null!;

    /// <summary>Отчество. Необязательное поле; при наличии — от 3 до 50 символов.</summary>
    /// <example>Сергеевич</example>
    [Column("father_name")]
    public string? FatherName { get; set; }

    /// <summary>Позывной поисковика. Необязательное поле.</summary>
    /// <example>Сокол</example>
    [Column("nick_name")]
    public string? NickName { get; set; }

    /// <summary>Контактный номер телефона. Обязательное поле.</summary>
    /// <example>+79991234567</example>
    [Column("phone_number")]
    public string PhoneNumber { get; set; } = null!;

    /// <summary>Группы, в которых состоит поисковик.</summary>
    public ICollection<GroupEntity> Groups { get; set; } = [];

    /// <inheritdoc />
    public override bool Validate(out string? errorMessage)
    {
        var sb = new StringBuilder();

        if (string.IsNullOrEmpty(LastName) || LastName.Length < 3 ||  LastName.Length > 50)
        {
            sb.AppendLine("Фамилия: обязательное поле, минимум символов 3, максимум символов 50");
        }
        if (string.IsNullOrEmpty(FirstName) || FirstName.Length < 3 ||  FirstName.Length > 50)
        {
            sb.AppendLine("Имя: обязательное поле, минимум символов 3, максимум символов 50");
        }
        if (FatherName?.Length is < 3 or > 50)
        {
            sb.AppendLine("Отчество: минимум символов 3, максимум символов 50");
        }

        errorMessage = sb.ToString();
        return string.IsNullOrEmpty(sb.ToString());
    }
}
