namespace RabbitMQBus.Event.IntegrationEventHandler
{
    public class TestProductIntegrationEventHandler : IIntegrationEventHandler<TestProduceIntegrationEvent>
    {
        public readonly IServiceProvider _serviceProvider;
        public TestProductIntegrationEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task Handle(TestProduceIntegrationEvent @event)
        {
            Console.WriteLine(@event.Message);

            return Task.FromResult(0);
        }
    }
}
