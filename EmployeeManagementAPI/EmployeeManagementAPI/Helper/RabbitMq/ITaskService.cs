using EmployeeManagement.Domain.Models.RabitMqModels;

namespace EmployeeManagement.Api.Helper.RabbitMq
{
    public interface ITaskService
    {
        Task HandleTaskAsync(TaskMessage taskMessageBase, CancellationToken cancellationToken);
    }
}
