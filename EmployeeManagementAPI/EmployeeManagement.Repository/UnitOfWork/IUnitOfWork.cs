using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagement.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagment.Services
{
    public interface IUnitOfWork
    {
        EmployeeRepository _employeeRepository { get; }
        Task<bool> Commit();
    }
}
