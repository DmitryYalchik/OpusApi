namespace OpusApi.DbModels;

/// <summary>
/// Активное подключение клиента к SignalR-хабу уведомлений.
/// </summary>
public class ConnectionEntity : IdentityModel
{
    /// <summary>Идентификаторы SignalR-соединений, связанных с пользователем.</summary>
    public List<string> ConnectionId { get; set; } = [];

    /// <summary>Имя пользователя, присвоенное подключению.</summary>
    /// <example>Оператор Иванов</example>
    public string? UserName { get; set; }

    /// <summary>Время установления подключения.</summary>
    public new DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <inheritdoc />
    public override bool Validate(out string? errorMessage)
    {
        errorMessage = null;
        return true;
    }
}
