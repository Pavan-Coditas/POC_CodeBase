using EmployeeManagement.Domain.Models.RabitMqModels;

namespace EmployeeManagement.Api.Helper.RabbitMq
{
    public interface ITaskHandler
    {
        Task HandleTaskAsync(TaskMessage taskMessage, CancellationToken cancellationToken);
    }
}
