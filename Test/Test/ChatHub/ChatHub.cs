using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public static Dictionary<string, string> connectedUsers = new Dictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        string userName = Context.GetHttpContext()!.Session.GetString("UserName")!;
        connectedUsers[Context.ConnectionId] = userName;
        await Clients.All.SendAsync("UserConnected", Context.ConnectionId, userName);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        connectedUsers.Remove(Context.ConnectionId);
        await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiverId, string message)
    {
        string senderId = Context.ConnectionId;
        string senderName = connectedUsers.FirstOrDefault(x => x.Key == senderId).Value;
        if (connectedUsers.ContainsKey(receiverId))
        {
            string receiverName = connectedUsers.FirstOrDefault(x => x.Key == receiverId).Value;
            await Clients.Client(receiverId).SendAsync("ReceiveMessage", senderName, senderId, message);
        }
        else
        {
            await Clients.Client(senderId).SendAsync("ReceiveMessage", "System", "The receiver is not online.");
        }
    }
}
