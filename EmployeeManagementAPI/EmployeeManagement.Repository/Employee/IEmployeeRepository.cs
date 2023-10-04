using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Repository
{
    public interface IEmployeeRepository<Entity, in TPK> where Entity : class
    {
        Entity CreateEmployee(Entity entity);
        Entity DeleteEmployee(int id);
        IEnumerable<Entity> GetAllEmployees();
        Entity GetEmployeeById(int id);
        Entity UpdateEmployee(Entity entity);
        Entity PatchEmployee(TPK id, JsonPatchDocument entity);
    }
}
