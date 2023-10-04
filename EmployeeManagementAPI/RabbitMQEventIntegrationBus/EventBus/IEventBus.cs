namespace RabbitMQBus
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event, bool isPriorityQueue = false) where T : IntegrationEvent;
        Task SubscribeAsync<T, TH>(CancellationToken cancellationToken) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
