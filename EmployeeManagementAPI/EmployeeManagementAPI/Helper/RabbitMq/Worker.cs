using EmployeeManagement.Domain.Models.RabitMqModels;

namespace EmployeeManagement.Api.Helper.RabbitMq
{
    public class Worker : BackgroundService
    {
        private readonly IBus _busControl;
        private readonly IObjectProvider _objectProvider;
        private readonly IConfiguration _configuration;

        public Worker(IObjectProvider objectProvider, IConfiguration configuration)
        {
            _objectProvider = objectProvider;
            _configuration = configuration;

            var config = new MsgConfig(_configuration);
            _busControl = RabbitHutch.CreateBus(config);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
         {
            await _busControl.ReceiveAsync<TaskMessage>(async (message, deliveryTag) =>
            {
                var cancellationTokenSource = new CancellationTokenSource();
                try
                {
                    var timerTask = Task.Delay(TimeSpan.FromSeconds(5 * 60));
                    var workerTask = TryHandleAsync(message, deliveryTag, cancellationTokenSource.Token);

                    var task = await Task.WhenAny(timerTask, workerTask);
                    if (task == timerTask)
                    {
                        cancellationTokenSource.Cancel();
                        if (message.Retry < message.MaxRetryCount)
                        {
                            message.Retry += 1;
                            await _busControl.SendAsync(Queue.Processing, message);
                        }
                    }
                }
                catch (OutOfMemoryException)
                {
                    await RetryAndRequeueAsync(message);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    _busControl.Ack(deliveryTag);
                }
            }, stoppingToken);
        }

        private async Task RetryAndRequeueAsync(TaskMessage message)
        {
            if (message.Retry < message.MaxRetryCount)
            {
                message.Retry += 1;
                await _busControl.SendAsync(Queue.Processing, message);
            }
        }

        private async Task TryHandleAsync(TaskMessage taskMessageBase, ulong deliveryTag, CancellationToken cancellationToken)
        {
            var diff = DateTime.Now - taskMessageBase.StartTime;
            if (diff.TotalMilliseconds > (15 * 60 * 1000))
            {
                _busControl.Ack(deliveryTag);
                return;
            }
            try
            {
                var taskHandler = _objectProvider.GetInstance<ITaskHandler>(taskMessageBase.MessageType.ToString());
                await taskHandler.HandleTaskAsync(taskMessageBase, cancellationToken);

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                _busControl.Ack(deliveryTag);
            }
        }
    }

}
