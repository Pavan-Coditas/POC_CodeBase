using Confluent.Kafka;
using System.Text.Json;

namespace RabbitMQEventIntegrationBus.KafkaConnection
{

    public class CustomSearilization<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            using (var ms = new MemoryStream())
            {
                string jsonString = JsonSerializer.Serialize(data);
                var writer = new StreamWriter(ms);

                writer.Write(jsonString);
                writer.Flush();
                ms.Position = 0;

                return ms.ToArray();
            }
        }
    }

    public class CustomDeSearilization<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<T>(data.ToArray());
        }
    }
}
