using Microsoft.AspNetCore.SignalR;

namespace OpusApi.Notifications;

/// <summary>
/// Реализация <see cref="IEntityNotifier"/> поверх SignalR-хаба уведомлений.
/// Все клиенты получают событие <c>EntityChanged</c> с полезной нагрузкой
/// <see cref="EntityChangeNotification"/>.
/// </summary>
public class SignalREntityNotifier(IHubContext<NotificationsHub> hub) : IEntityNotifier
{
    private const string ClientMethod = "EntityChanged";

    public Task EntityCreatedAsync(string entity, Guid id) =>
        NotifyAsync(entity, "created", id);

    public Task EntityUpdatedAsync(string entity, Guid id) =>
        NotifyAsync(entity, "updated", id);

    public Task EntityDeletedAsync(string entity, Guid id) =>
        NotifyAsync(entity, "deleted", id);

    private Task NotifyAsync(string entity, string action, Guid id) =>
        hub.Clients.All.SendAsync(ClientMethod, new EntityChangeNotification(entity, action, id));
}
