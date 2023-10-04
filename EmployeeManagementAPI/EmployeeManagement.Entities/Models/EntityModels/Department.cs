using System.Text.Json.Serialization;

namespace EmployeeManagement.Entities.Models.EntityModels
{
    public partial class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Employee> Employees { get; set; }

    }
}
