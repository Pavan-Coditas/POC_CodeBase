using System.ComponentModel.DataAnnotations;

namespace EmployeeApiConsumer.Models
{
    public class CreateNewEmployee
    {
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? GenderId { get; set; }
        public string? GenderName { get; set; }
    }
}
