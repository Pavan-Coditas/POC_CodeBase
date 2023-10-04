using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EmployeeManagement.Api.Helper.RabbitMq
{
    public class MsgConfig
    {
        private readonly IConfiguration configProvider;
        public MsgConfig(IConfiguration configProvider)
        {
            this.configProvider = configProvider;
        }
        public string GetUri()
        {
            return configProvider.GetValue<string>("rabbitMq:Uri")!;
        }
        public string UserName()
        {
            return configProvider.GetValue<string>("rabbitMq:user")!;
        }
        public string GetPassword()
        {
            return configProvider.GetValue<string>("rabbitMq:password")!;
        }
    }

    public class RabbitBus : IBus
    {
        private readonly IModel _channel;
        internal RabbitBus(IModel channel)
        {
            _channel = channel;
            _channel.BasicQos(0, 1, true);
        }

        public void Publish<T>(string queue, string exchangeName, string routingKey, T message)
        {
            _channel.QueueDeclare(queue, false, false, false);
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueBind(queue, exchangeName, routingKey, null);
            var output = JsonConvert.SerializeObject(message);
            _channel.BasicPublish(exchangeName, routingKey, null, Encoding.UTF8.GetBytes(output));
        }

        public async Task ReceiveAsync<T>(string queue, string exchangeName, string routingKey, T message)
        {
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queue, false, false, false);
            _channel.QueueBind(queue, exchangeName, routingKey, null);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) =>
            {
                var jsonSpecified = Encoding.UTF8.GetString(args.Body.Span);
                var item = JsonConvert.DeserializeObject<T>(jsonSpecified);
                await Task.Yield();
            };
            var consumerTag = _channel.BasicConsume(queue, false, consumer);
            _channel.BasicCancel(consumerTag);
            await Task.Yield();
        }
    }

    public interface IBus
    {
        void Publish<T>(string queue, string exchangeName, string routingKey, T message);
        Task ReceiveAsync<T>(string queue, string exchangeName, string routingKey, T message);
    }
    public class Queue
    {
        public static string QueueName { get; } = "EventQueue";
        public static string ExachngeName { get; } = "DemoExchange";
        public static string RoutingKey { get; } = "Demo-Routing-Key";
    }
    public class RabbitHutch
    {
        private static ConnectionFactory? _factory;
        private static IConnection? _connection;
        private static IModel? _channel;

        public static IBus CreateBus(MsgConfig config)
        {
            _factory = new ConnectionFactory
            {
                Uri = new Uri(config.GetUri()),
                UserName = config.UserName(),
                Password = config.GetPassword(),
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            return new RabbitBus(_channel);
        }
    }
}
