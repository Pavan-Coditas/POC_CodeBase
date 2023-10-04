namespace EmployeeManagement.Entities.Models.EntityModels
{
    public partial class User
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int GenderId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public string? CreatedBy { get; set; }
    }
}
