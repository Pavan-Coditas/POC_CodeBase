using Microsoft.Extensions.Hosting;
using RabbitMQBus;

namespace ConnectAndSell.Common.EventBus
{
    public class AMQPWorker : BackgroundService
    {
        private readonly IEventBus _eventBus;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Action<IEventBus> _eventAction = null!;

        public AMQPWorker(IEventBus eventBus, Action<IEventBus> eventAction)
        {
            _eventAction = eventAction;
            _eventBus = eventBus;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _eventAction(_eventBus);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public void StopWorker()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
