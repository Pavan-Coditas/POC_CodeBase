using System;
using System.Collections.Generic;

namespace EmployeeManagement.Entities.Models.EntityModels
{
    public partial class RoleMapping
    {
        public int? RoleId { get; set; }
        public int? UserId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual User? User { get; set; }
    }
}
