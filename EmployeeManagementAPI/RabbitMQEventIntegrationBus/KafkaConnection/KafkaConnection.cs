using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace RabbitMQEventIntegrationBus.KafkaConnection
{
    public class KafkaConnection
    {
        private readonly ClientConfig config;
        private readonly SchemaRegistryConfig _schemaRegistryConfiguration;
        private object _producerBuilder = null!;

        public KafkaConnection(ClientConfig producerConfig,
            SchemaRegistryConfig schemaRegistryConfig, AvroSerializerConfig avroSerializerConfig)
        {

            this.config = producerConfig ?? throw new ArgumentNullException(nameof(producerConfig));
            this._schemaRegistryConfiguration = schemaRegistryConfig ?? throw new ArgumentNullException(nameof(schemaRegistryConfig));

        }

        public IProducer<Null, T> ProducerBuilder<T>()
        {
            if (_producerBuilder == null)
            {
                var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfiguration);
                _producerBuilder = new ProducerBuilder<Null, T>(config)
                             //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                             .SetValueSerializer(new CustomSearilization<T>())
                            .Build();
            }
            return (IProducer<Null, T>)_producerBuilder;
        }

        public IConsumer<Null, T> ConsumerBuilder<T>()
        {
            var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfiguration);
            var consumerConfig = new ConsumerConfig(config);
            consumerConfig.GroupId = "test-group-1";
            // The offset to start reading from if there are no committed offsets (or there was an error in retrieving offsets).
            consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
            // Do not commit offsets.
            consumerConfig.EnableAutoCommit = false;
            var consumer = new ConsumerBuilder<Null, T>(consumerConfig)
                //.SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                .SetValueDeserializer(new CustomDeSearilization<T>())
                .Build();
            return consumer;
        }


    }
}
