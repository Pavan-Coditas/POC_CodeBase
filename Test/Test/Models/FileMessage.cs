namespace EmployeeApiConsumer.Models
{
    public class FileMessage
    {
        public byte[]? FileBinary { get; set; }
        public string FileHeaders { get; set; } = string.Empty;
    }
}
