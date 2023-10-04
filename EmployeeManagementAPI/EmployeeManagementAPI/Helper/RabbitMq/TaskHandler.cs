using EmployeeManagement.Domain.Models.RabitMqModels;

namespace EmployeeManagement.Api.Helper.RabbitMq
{
    public abstract class TaskHandler : ITaskHandler
    {
        private readonly IBus _bus;
        //protected readonly string _queueName;

        public TaskHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task HandleTaskAsync(TaskMessage taskMessage, CancellationToken cancellationToken)
        {
            try
            {
                // Resolve handler
                var result = await HandleAsync(taskMessage, cancellationToken);

                // Publish a message to RabbitMQ to indicate task completion
                _bus.SendAsync(Queue.QueueName,Queue.ExachngeName,Queue.RoutingKey,taskMessage);
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
            }
        }
        public abstract Task<object> HandleAsync(TaskMessage taskMessage, CancellationToken cancellationToken);
    }

    public class CustomTaskHandler : TaskHandler
    {
        private readonly IObjectProvider _objectProvider;
        public CustomTaskHandler(IBus bus, IObjectProvider objectProvider) : base(bus)
        {
            _objectProvider = objectProvider;
        }

        public override async Task<object> HandleAsync(TaskMessage taskMessage, CancellationToken cancellationToken)
        {
            var obj = _objectProvider.GetInstance<ITaskHandler>(TaskMessage.TaskMessageType.CustomTaskHandler.ToString());
            return new object();
        }
    }

    public class TicketTaskHandler : TaskHandler
    {
        private readonly IObjectProvider objectProvider;

        public TicketTaskHandler(IBus bus, IObjectProvider objectProvider)
            : base(bus)
        {
            this.objectProvider = objectProvider;
        }

        public override async Task<object> HandleAsync(TaskMessage taskMessage, CancellationToken cancellationToken)
        {
            var obj = objectProvider.GetInstance<ITaskHandler>(TaskMessage.TaskMessageType.TicketTaskHandler.ToString());
            return new object();
        }
    }

    public class UnknownTaskHandler : TaskHandler
    {
        private readonly IObjectProvider objectProvider;

        public UnknownTaskHandler(IObjectProvider objectProvider, IBus bus)
            : base(bus)
        {
            this.objectProvider = objectProvider;
        }

        public override async Task<object> HandleAsync(TaskMessage taskMessage, CancellationToken cancellationToken)
        {
            var obj = objectProvider.GetInstance<ITaskHandler>(TaskMessage.TaskMessageType.UnknownTaskHandlers.ToString());
            return new object();
        }
    }
}
