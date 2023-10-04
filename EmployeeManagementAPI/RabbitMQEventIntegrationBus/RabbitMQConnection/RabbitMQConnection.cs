using RabbitMQ.Client;

namespace RabbitMQBus.RabbitMqConnection
{
    public class RabbitMQConnection
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection? _connection;
        public RabbitMQConnection()
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")!,
                UserName = "guest",
                Password = "guest"
            };
        }

        public IModel CreateModel()
        {
            if (_connection == null || !_connection.IsOpen)
                _connection = _connectionFactory.CreateConnection();

            return _connection.CreateModel();
        }

        public void Close()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
