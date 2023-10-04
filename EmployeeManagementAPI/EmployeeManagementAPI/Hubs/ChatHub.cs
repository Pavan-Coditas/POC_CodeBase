using Microsoft.AspNetCore.SignalR;

namespace EmployeeManagement.Api.Hubs
{

    public class ChatHub : Hub
    {
       public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }
        /*public Task SendMessageToGroup(string sender, string receiver, string message)
        {
            //message send to receiver only
            return Clients.Group(receiver).SendAsync("ReceiveMessage", sender, message);
        }*/
    }
}
