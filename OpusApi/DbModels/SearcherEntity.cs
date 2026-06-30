using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OpusApi.DbModels;

[Table("searcher_entity")]
public class SearcherEntity : IdentityModel
{
    [Column("last_name")]
    public string LastName { get; set; } = null!;
    
    [Column("first_name")]
    public string FirstName { get; set; } = null!;
    
    [Column("father_name")]
    public string? FatherName { get; set; }
    
    [Column("nick_name")]
    public string? NickName { get; set; }
    
    [Column("phone_number")]
    public string PhoneNumber { get; set; } = null!;

    public IEnumerable<GroupEntity> Groups { get; set; } = [];
    
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