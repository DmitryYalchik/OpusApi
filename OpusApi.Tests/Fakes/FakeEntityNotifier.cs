using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpusApi.Notifications;

namespace OpusApi.Tests.Fakes;

/// <summary>Запись об отправленном уведомлении.</summary>
public record SentNotification(string Entity, string Action, Guid Id);

/// <summary>
/// Тестовая реализация <see cref="IEntityNotifier"/>: ничего не рассылает,
/// а только запоминает вызовы, чтобы тесты могли их проверить.
/// </summary>
public class FakeEntityNotifier : IEntityNotifier
{
    public List<SentNotification> Sent { get; } = [];

    public Task EntityCreatedAsync(string entity, Guid id) => Record(entity, "created", id);
    public Task EntityUpdatedAsync(string entity, Guid id) => Record(entity, "updated", id);
    public Task EntityDeletedAsync(string entity, Guid id) => Record(entity, "deleted", id);

    private Task Record(string entity, string action, Guid id)
    {
        Sent.Add(new SentNotification(entity, action, id));
        return Task.CompletedTask;
    }
}
