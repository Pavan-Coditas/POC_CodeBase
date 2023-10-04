namespace EmployeeManagement.Api.Helper.RabbitMq
{
    public interface IObjectProvider
    {
        T GetInstance<T>(string key);
    }
}
