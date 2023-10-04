using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQBus;

namespace RabbitMQEventIntegrationBus.KafkaConnection
{
    public class KafkaEventBus : IEventBus
    {
        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly KafkaConnection _kafkaConnection;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ClientConfig _clientConfig;
        public KafkaEventBus(IEventBusSubscriptionManager subscriptionManager,
            KafkaConnection kafkaConnection, IServiceProvider serviceProvider, ClientConfig config)
        {
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            _kafkaConnection = kafkaConnection ?? throw new ArgumentNullException(nameof(kafkaConnection));
            _serviceScopeFactory = serviceProvider?.GetRequiredService<IServiceScopeFactory>()
                ?? throw new ArgumentException($"Cannot resolve IServiceScopeFactory from {nameof(serviceProvider)}");
            _clientConfig = config;
        }

        public async Task PublishAsync<T>(T @event, bool isPriorityQueue = false) where T : IntegrationEvent
        {
            var eventType = typeof(T).Name;
            try
            {
                await CreateTopic(eventType, _clientConfig);
                var producer = _kafkaConnection.ProducerBuilder<T>();
                var producerResult = await producer.ProduceAsync(eventType, new Message<Null, T>() { Value = @event });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateTopic(string topicName, ClientConfig config)
        {
            using (var adminClient = new AdminClientBuilder(config).Build())
            {
                var topicMetadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));

                if (!topicMetadata.Topics.Any(t => t.Topic.Equals(topicName, StringComparison.OrdinalIgnoreCase)))
                {
                    var topicSpecs = new TopicSpecification[]
                    {
                    new TopicSpecification
                    {
                        Name = topicName,
                        NumPartitions = 6,
                        ReplicationFactor = 3
                    }
                    };

                    await adminClient.CreateTopicsAsync(topicSpecs);
                }
            }
        }

        public async Task SubscribeAsync<T, TH>(CancellationToken cancellationToken)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            using (var consumer = _kafkaConnection.ConsumerBuilder<T>())
            {
                //subscribe the handler to the event
                _subscriptionManager.AddSubscription<T, TH>();

                consumer.Subscribe(eventName);
                Console.WriteLine($"Subscribed to event '{eventName}' from Kafka");
                //create a task to listen to the topic
                await Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            var consumerResult = consumer.Consume();
                            await ProcessEvent(consumerResult.Message.Value);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }).ConfigureAwait(false);
            }
        }

        private async Task<bool> ProcessEvent<T>(T value) where T : IntegrationEvent
        {
            var processed = false;
            if (_subscriptionManager.HasEvent<T>())
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var subscriptions = _subscriptionManager.GetHandlersForEvent<T>();
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetRequiredService(subscription);
                        if (handler == null) continue;
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(typeof(T));
                        await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new object[] { value })!;
                    }
                }
                processed = true;
                Console.WriteLine($"Processed event of type '{typeof(T).Name}'");
            }
            return processed;
        }
    }
}
