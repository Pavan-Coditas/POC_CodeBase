namespace EmployeeManagment.Services
{
    public interface IUnitOfWork
    {
        EmployeeRepository _employeeRepository { get; }
        Task<bool> Commit();
    }
}
