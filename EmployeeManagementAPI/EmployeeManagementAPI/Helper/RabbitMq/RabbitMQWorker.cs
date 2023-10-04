using EmployeeManagement.Domain.Models.RabitMqModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMQWorker : IDisposable
{
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly Dictionary<string, Action<string>> _eventHandlers;

    public RabbitMQWorker(IModel channel, string queueName)
    {
        _channel = channel;
        _queueName = queueName;
        _eventHandlers = new Dictionary<string, Action<string>>
        {
            { EventType.Create.ToString(), ProcessCreateEvent },
            { EventType.Update.ToString(), ProcessUpdateEvent},
            { EventType.Delete.ToString(), ProcessDeleteEvent }
            // Add more event types and corresponding event handling methods here
        };
    }

    public void StartListening()
    {
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, args) =>
        {
            var message = Encoding.UTF8.GetString(args.Body.ToArray());
            var MessageObject = JsonConvert.DeserializeObject<TaskMessage>(message);
            //Console.WriteLine($"Received message: {message}");
            if (_eventHandlers.TryGetValue(MessageObject!.EventType.ToString(), out var eventHandler))
            {
                eventHandler.Invoke(message);
            }
            else
            {
                Console.WriteLine("Unknown event type: {0}", MessageObject.EventType);
            }

            _channel.BasicAck(args.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
    }

    private void ProcessUpdateEvent(string eventData)
    {
        Console.WriteLine("Update event processed: {0}", eventData);
    }

    private void ProcessCreateEvent(string eventData)
    {
        Console.WriteLine("Create event processed: {0}", eventData);
    }

    private void ProcessDeleteEvent(string eventData)
    {
        Console.WriteLine("Delete event processed: {0}", eventData);
    }

    public void Dispose()
    {
        _channel.Close();
    }
}
