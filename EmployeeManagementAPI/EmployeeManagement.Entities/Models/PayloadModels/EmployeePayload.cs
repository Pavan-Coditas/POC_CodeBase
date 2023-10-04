namespace EmployeeManagement.Entities.Models.PayloadModel
{
    public partial class Employeepayload
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public decimal Salary { get; set; }
        public string Designation { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public int DeptId { get; set; }
        public int? GenderId { get; set; }
    }
}
