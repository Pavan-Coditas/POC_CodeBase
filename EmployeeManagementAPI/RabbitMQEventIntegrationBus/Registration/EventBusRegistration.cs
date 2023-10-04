using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQBus;
using RabbitMQEventIntegrationBus.KafkaConnection;

namespace ConnectAndSell.Common.EventBus
{
    public static class EventBusRegistration
    {

        public static IServiceCollection AddEventBusSubscriptionManager(this IServiceCollection services)
        {
            // Add services to the container.
            services.AddSingleton<IEventBusSubscriptionManager, EventBusSubscriptionManager>();
            return services;
        }

        public static IServiceCollection RegisterWorker(this IServiceCollection services)
        {
            var config = new ClientConfig
            {
                BootstrapServers = "pkc-6ojv2.us-west4.gcp.confluent.cloud:9092",
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "2DB4ZOUIDVP2YDQ3",
                SaslPassword = "mrDuZecS9Fse7p2/K8HHdEVUutHFsCkwTfUV3KJcG8BBamvsR9xKr3c2EEHjB3Cs",
                AllowAutoCreateTopics = true,
                SslCaLocation = "probe"
            };

            var schemaRegistryConfiguration = new Confluent.SchemaRegistry.SchemaRegistryConfig
            {
                Url = "https://pkc-6ojv2.us-west4.gcp.confluent.cloud:443",
                BasicAuthCredentialsSource = Confluent.SchemaRegistry.AuthCredentialsSource.UserInfo,
                BasicAuthUserInfo = "2DB4ZOUIDVP2YDQ3:mrDuZecS9Fse7p2/K8HHdEVUutHFsCkwTfUV3KJcG8BBamvsR9xKr3c2EEHjB3Cs"
                //RequestTimeoutMs = 5000,
                //MaxCachedSchemas = 10,
            };

            var avroSerializerConfiguration = new AvroSerializerConfig
            {
                AutoRegisterSchemas = true
            };
            // Set up the event bus
            services.AddSingleton<KafkaConnection>(new KafkaConnection(
                config,
                schemaRegistryConfiguration,
                avroSerializerConfiguration));
            services.AddSingleton<IEventBusSubscriptionManager, EventBusSubscriptionManager>();

            services.AddSingleton<IEventBus, KafkaEventBus>(sp =>
            {
                var kafkaConnection = sp.GetRequiredService<KafkaConnection>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                return new KafkaEventBus(eventBusSubscriptionsManager, kafkaConnection, sp, config);
            });

            services.AddHostedService(serviceProvider =>
            new AMQPWorker(serviceProvider.GetRequiredService<IEventBus>(), SubscribeToEvents));
            return services;

        }

        public async static void SubscribeToEvents(IEventBus eventBus)
        {
            var subscription1Task = eventBus.SubscribeAsync<TestProduceIntegrationEvent, IIntegrationEventHandler<TestProduceIntegrationEvent>>(new CancellationToken());
            await Task.WhenAll(subscription1Task);
        }
        public static IServiceCollection RegisterEventHandlers(this IServiceCollection services)
        {
            return services;
        }
    }
}
