using EmployeeManagement.Entities.Models.EntityModels;
using System;
using System.Collections.Generic;

namespace EmployeeManagement.Entities.Models.DTOModels
{
    public partial class DepartmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
