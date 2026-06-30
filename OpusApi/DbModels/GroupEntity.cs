using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OpusApi.DbModels;

/// <summary>
/// Группа — объединение поисковиков на поиске.
/// </summary>
[Table("groups")]
public class GroupEntity : IdentityModel
{
    /// <summary>Название группы. Обязательное поле, максимум 50 символов.</summary>
    /// <example>Группа №1</example>
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Поисковики, входящие в группу.</summary>
    public ICollection<SearcherEntity> Searchers { get; set; } = [];

    /// <inheritdoc />
    public override bool Validate(out string? errorMessage)
    {
        var sb = new StringBuilder();

        if (string.IsNullOrEmpty(Name) ||  Name.Length > 50)
        {
            sb.AppendLine("Название: обязательное поле, максимум символов 50");
        }

        errorMessage = sb.ToString();
        return string.IsNullOrEmpty(sb.ToString());
    }
}
