namespace EmployeeManagment.Services
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GetUnitOfWork();
        IUnitOfWork GetUnitOfWork(DbOperation operation);
    }
}
