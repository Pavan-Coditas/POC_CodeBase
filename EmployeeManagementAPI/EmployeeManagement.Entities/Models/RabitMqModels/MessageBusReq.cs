namespace EmployeeManagement.Domain.Models.RabitMqModels
{
    public class MessageBusReq
    {
        public int Retry { get; set; } = 1;
        public int MaxRetryCount { get; set; } = 3;
        public enum TaskMessageType { CustomTaskHandler, TicketTaskHandler, UnknownTaskHandlers }
    }
}
