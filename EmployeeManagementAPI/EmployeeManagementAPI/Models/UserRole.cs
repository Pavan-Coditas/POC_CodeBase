using System;
using System.Collections.Generic;

namespace EmployeeManagement.Api.Models
{
    public partial class UserRole
    {
        public string? RoleId { get; set; }
        public string? UserId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual User? User { get; set; }
    }
}
