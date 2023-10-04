using EmployeeManagement.Domain.Models.RabitMqModels;

namespace EmployeeManagement.Api.Helper.RabbitMq
{
    public class TaskService : ITaskService
    {
        private readonly IObjectProvider _objectProvider;
        private readonly IBus _bus;
        public TaskService(IObjectProvider objectProvider, IBus bus)
        {
            _objectProvider = objectProvider;
            _bus = bus;
        }

        public async Task HandleTaskAsync(TaskMessage taskMessageBase, CancellationToken cancellationToken)
        {
            var taskHandler = _objectProvider.GetInstance<ITaskHandler>(taskMessageBase.MessageType.ToString());

            var cancellationTokenSource = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                await taskHandler.HandleTaskAsync(taskMessageBase, cancellationTokenSource.Token);

                // Get the next task message from RabbitMQ and start handling it
            });

            await Task.CompletedTask; // Wait for the task to complete
        }
    }

}
