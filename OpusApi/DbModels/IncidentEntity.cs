using System.Text;

namespace OpusApi.DbModels;

public class IncidentEntity : IdentityModel
{
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public string Message { get; set; } = null!;

    public override bool Validate(out string? errorMessage)
    {
        var sb = new StringBuilder();
        
        if (string.IsNullOrEmpty(From) ||  From.Length > 50)
        {
            sb.AppendLine("Отправитель: обязательное поле, максимум символов 50");
        }
        if (string.IsNullOrEmpty(From) ||  From.Length > 50)
        {
            sb.AppendLine("Получатель: обязательное поле, максимум символов 50");
        }
        if (string.IsNullOrEmpty(From) ||  From.Length > 350)
        {
            sb.AppendLine("Сообщение: обязательное поле, максимум символов 350");
        }
        
        errorMessage = sb.ToString();
        return string.IsNullOrEmpty(sb.ToString());
    }
}