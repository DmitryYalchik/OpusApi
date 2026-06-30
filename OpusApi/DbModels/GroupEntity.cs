using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OpusApi.DbModels;

[Table("groups")]
public class GroupEntity : IdentityModel
{
    [Column("name")]
    public string Name { get; set; }

    public IEnumerable<SearcherEntity> Searchers { get; set; } = [];
    
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