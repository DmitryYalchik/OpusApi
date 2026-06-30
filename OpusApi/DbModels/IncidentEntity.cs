using System.Text;

namespace OpusApi.DbModels;

/// <summary>
/// Запись журнала связи — зафиксированное сообщение между участниками поиска.
/// </summary>
public class IncidentEntity : IdentityModel
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

    /// <inheritdoc />
    public override bool Validate(out string? errorMessage)
    {
        var sb = new StringBuilder();

        if (string.IsNullOrEmpty(From) || From.Length > 50)
        {
            sb.AppendLine("Отправитель: обязательное поле, максимум символов 50");
        }
        if (string.IsNullOrEmpty(To) || To.Length > 50)
        {
            sb.AppendLine("Получатель: обязательное поле, максимум символов 50");
        }
        if (string.IsNullOrEmpty(Message) || Message.Length > 350)
        {
            sb.AppendLine("Сообщение: обязательное поле, максимум символов 350");
        }

        errorMessage = sb.ToString();
        return string.IsNullOrEmpty(sb.ToString());
    }
}
