using EmployeeManagement.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EmployeeManagment.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private IDbContextTransaction? _transaction;
        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new GenericRepository<TEntity>(_context);
        }

        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }

        public void BeginTransaction()
        {
            _transaction ??= _context.Database.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            _transaction!.Rollback();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        if (CommitFailed())
                        {
                            RollbackTransaction();
                        }
                        _transaction.Dispose();
                    }
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool CommitFailed()
        {
            // Disable entity tracking to avoid circular references
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            // Check if there are any entities in the context with state Added, Modified, or Deleted
            bool hasChanges = _context.ChangeTracker.Entries().Any(e =>
                e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            // Check if the database can connect
            bool canConnect = false;
            try
            {
                canConnect = _context.Database.CanConnect();
            }
            catch (Exception ex)
            {
                // Database connection failed
                throw new Exception($"Something went wrong{ex.Message}");
            }

            return hasChanges && !canConnect;
        }

    }
}