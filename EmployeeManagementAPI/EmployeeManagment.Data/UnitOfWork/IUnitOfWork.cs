using EmployeeManagement.Repository.Generic;

namespace EmployeeManagment.Services
{
    public interface IUnitOfWork : IDisposable
    {
        bool Commit();
        void BeginTransaction();
        void RollbackTransaction();
        bool CommitFailed();
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}
