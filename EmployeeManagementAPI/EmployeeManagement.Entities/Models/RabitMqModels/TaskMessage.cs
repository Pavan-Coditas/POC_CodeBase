namespace EmployeeManagement.Domain.Models.RabitMqModels
{
    public class TaskMessage
    {
        public string Key { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public EventType EventType { get; set; }
    }

    public enum EventType
    {
        Create,
        Update,
        Delete,
    }
}
