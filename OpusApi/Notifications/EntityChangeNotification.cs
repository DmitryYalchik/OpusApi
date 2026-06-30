namespace OpusApi.Notifications;

/// <summary>
/// Уведомление об изменении записи, рассылаемое подключённым клиентам через SignalR.
/// </summary>
/// <param name="Entity">Тип сущности: <c>Searcher</c>, <c>Group</c> или <c>Incident</c>.</param>
/// <param name="Action">Действие: <c>created</c>, <c>updated</c> или <c>deleted</c>.</param>
/// <param name="Id">Идентификатор затронутой записи.</param>
public record EntityChangeNotification(string Entity, string Action, Guid Id);
