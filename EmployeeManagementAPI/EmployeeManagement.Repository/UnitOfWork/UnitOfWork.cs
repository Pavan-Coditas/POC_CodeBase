using EmployeeManagement.Domain;
using EmployeeManagment.Services;

namespace RepositoryUsingEFinMVC.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EmployeeManagementContext _context;
        public UnitOfWork(EmployeeManagementContext context)
        {
            _context = context;
        }
        public EmployeeRepository _employeeRepository => new EmployeeRepository(_context);

        public async Task<bool> Commit()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }

}