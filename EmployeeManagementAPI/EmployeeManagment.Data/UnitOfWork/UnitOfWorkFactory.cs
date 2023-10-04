using EmployeeManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagment.Services
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly DbContextOptions<EmployeeManagementContext> _dbContextOptions;

        public UnitOfWorkFactory(DbContextOptions<EmployeeManagementContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public IUnitOfWork GetUnitOfWork()
        {

            var context = new EmployeeManagementContext(_dbContextOptions);
            return new UnitOfWork(context);
        }

        public IUnitOfWork GetUnitOfWork(DbOperation operation)
        {
            var unitOfWork = GetUnitOfWork();
            if (operation == DbOperation.Write)
            {
                unitOfWork.BeginTransaction();
            }
            return unitOfWork;
        }
    }
}
