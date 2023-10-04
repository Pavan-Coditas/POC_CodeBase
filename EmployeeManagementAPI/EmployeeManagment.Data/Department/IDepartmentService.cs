using EmployeeManagement.Entities.Models.EntityModels;

namespace EmployeeManagment.Services
{
    public interface IDepartmentService
    {
        IEnumerable<Department> GetAll();
    }
}
