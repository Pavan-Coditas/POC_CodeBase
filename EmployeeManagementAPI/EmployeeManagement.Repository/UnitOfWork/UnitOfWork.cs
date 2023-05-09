using EmployeeManagment.Services;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using EmployeeManagement.Repository;
using EmployeeManagement.Domain;

namespace RepositoryUsingEFinMVC.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EmployeeManagementContext _context;
        public UnitOfWork(EmployeeManagementContext context)
        {
            _context= context;
        }
        public EmployeeRepository _employeeRepository => new EmployeeRepository(_context);

        public async Task<bool> Commit()
        {
            return  await _context.SaveChangesAsync() > 0;
        }
    }

}