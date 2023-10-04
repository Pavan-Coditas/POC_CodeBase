using Microsoft.AspNetCore.SignalR;

namespace EmployeeMangementApi.Hub.Hub
{
    public class NotificationHub : Hub<INotificationHubClient>
    {
        public async Task SendNotificationToUser(string message)
        {
            await Clients.All.SendNotificationToUser(message);
        }
    }
}
