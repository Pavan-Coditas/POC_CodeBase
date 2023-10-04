using EmployeeManagement.Entities.Models.DTOModels;
using EmployeeManagement.Entities.Models.EntityModels;
using Microsoft.AspNetCore.JsonPatch;

namespace EmployeeManagment.Services
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> GetAll();
        Employee Get(int id);
        Employee Create(EmployeeDTO entity);
        Employee Update(EmployeeDTO entity);
        Employee Delete(int id);
        Employee Patch(JsonPatchDocument entity, int id);
    }
}
