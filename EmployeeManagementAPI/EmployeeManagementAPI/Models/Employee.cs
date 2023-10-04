using System;
using System.Collections.Generic;

namespace EmployeeManagement.Api.Models
{
    public partial class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public decimal Salary { get; set; }
        public string Designation { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public int DeptId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public string? CreatedBy { get; set; }
        public int? GenderId { get; set; }

        public virtual Department Dept { get; set; } = null!;
        public virtual Gender? Gender { get; set; }
    }
}
