namespace EmployeeManagement.Entities.Models.EntityModels
{
    public partial class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? Experience { get; set; }
        public int? Salary { get; set; }
    }
}
