namespace OpusApi.Notifications;

/// <summary>
/// Рассылка подключённым клиентам уведомлений о добавлении, изменении и удалении записей.
/// </summary>
public interface IEntityNotifier
{
    /// <summary>Уведомить о создании записи.</summary>
    Task EntityCreatedAsync(string entity, Guid id);

    /// <summary>Уведомить об изменении записи.</summary>
    Task EntityUpdatedAsync(string entity, Guid id);

    /// <summary>Уведомить об удалении записи.</summary>
    Task EntityDeletedAsync(string entity, Guid id);
}
