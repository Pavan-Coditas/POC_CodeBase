using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQBus.RabbitMqConnection
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly RabbitMQConnection _rabbitMQConnection;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQEventBus(IEventBusSubscriptionManager subscriptionManager,
            RabbitMQConnection rabbitMQConnection, IServiceProvider serviceProvider)
        {
            _subscriptionManager = subscriptionManager;
            _rabbitMQConnection = rabbitMQConnection;
            _serviceScopeFactory = serviceProvider?.GetRequiredService<IServiceScopeFactory>()!;
        }

        public Task PublishAsync<T>(T @event, bool isPriorityQueue = false) where T : IntegrationEvent
        {
            try
            {
                var eventType = typeof(T).Name;

                using (var channel = _rabbitMQConnection.CreateModel())
                {
                    channel.ExchangeDeclare("event-exchange", "x-delayed-message", durable: true, arguments: new Dictionary<string, object>()
                    {
                        { "x-delayed-type", "direct" }
                    });

                    // Generate a unique queue name for the event type
                    var queueName = $"{eventType}-queue";
                    channel.QueueDeclare(queueName, true, false, false, null);
                    channel.QueueBind(queueName, "event-exchange", eventType, null);

                    var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
                    var properties = channel.CreateBasicProperties();
                    if (isPriorityQueue)
                    {
                        properties.Priority = 10;
                    }
                    properties.Persistent = true; // Set message persistence
                    properties.Headers = new Dictionary<string, object>()
                    {
                        { "x-delay", 15000 }
                    };
                    // Publish the message to the dynamically generated queue
                    channel.BasicPublish("event-exchange", eventType, properties, message);

                    Console.WriteLine($"Published event '{eventType}' to RabbitMQ");
                    return Task.CompletedTask;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SubscribeAsync<T, TH>(CancellationToken cancellationToken) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {

            var eventName = typeof(T).Name;
            //int batchSize = 4;

            using (var channel = _rabbitMQConnection.CreateModel())
            {
                channel.ExchangeDeclare("event-exchange", ExchangeType.Direct);

                var queueName = $"{eventName}-queue";
                channel.QueueDeclare(queueName, true, false, false, null);
                channel.QueueBind(queueName, "event-exchange", eventName, null);

                _subscriptionManager.AddSubscription<T, TH>();

                var consumer = new EventingBasicConsumer(channel);


                consumer.Received += async (sender, args) =>
                {

                    var message = Encoding.UTF8.GetString(args.Body.ToArray());
                    await ProcessEvent<T>(message);
                    channel.BasicAck(args.DeliveryTag, false);

                };
                //channel.BasicQos(0, batchSize, false);


                channel.BasicConsume(queueName, false, consumer);

                // Consume messages from the dynamically generated queue

                Console.WriteLine($"Subscribed to event '{eventName}' from RabbitMQ");

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Keep the consumer listening by delaying for a short interval
                    await Task.Delay(100, cancellationToken);
                }
            }


        }

        private async Task ProcessEvent<T>(string message) where T : IntegrationEvent
        {
            try
            {
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
                            await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new object[] { JsonConvert.DeserializeObject<T>(message)! })!;
                        }
                    }
                }
                Console.WriteLine($"Processed event of type '{typeof(T).Name}'");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
