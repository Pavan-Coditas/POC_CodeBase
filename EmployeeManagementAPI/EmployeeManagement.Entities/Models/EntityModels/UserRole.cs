namespace EmployeeManagement.Entities.Models.EntityModels
{
    public partial class UserRole
    {
        public string? RoleId { get; set; }
        public string? UserId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual User? User { get; set; }
    }
}
