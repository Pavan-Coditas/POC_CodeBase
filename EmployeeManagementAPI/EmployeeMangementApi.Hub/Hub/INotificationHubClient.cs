namespace EmployeeMangementApi.Hub.Hub
{
    public interface INotificationHubClient
    {
        Task SendNotificationToUser(string message);
    }
}
