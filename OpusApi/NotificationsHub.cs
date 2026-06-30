using Microsoft.AspNetCore.SignalR;
using OpusApi.DbModels;

namespace OpusApi;

public class NotificationsHub(InMemoryDbContext dbContext) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var connection = new ConnectionEntity();
        connection.ConnectionId.Add(Context.ConnectionId);

        dbContext.Connections.Add(connection);
        await dbContext.SaveChangesAsync();

        await base.OnConnectedAsync();
    }

    public async Task JoinHub(string name)
    {
        if (dbContext.Connections.Any(x => x.ConnectionId.Contains(Context.ConnectionId)))
        {
            var connection = dbContext.Connections.First(x => x.ConnectionId.Contains(Context.ConnectionId));
            connection.UserName = name;
            connection.ConnectionId.Add(Context.ConnectionId);
        }
        else
        {
            var connection = new ConnectionEntity
            {
                UserName = name,
                ConnectionId = [Context.ConnectionId]
            };
            dbContext.Connections.Add(connection);
        }

        await dbContext.SaveChangesAsync();
    }
}