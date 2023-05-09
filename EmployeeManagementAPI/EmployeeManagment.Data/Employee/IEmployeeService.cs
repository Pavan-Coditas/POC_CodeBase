using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagment.Services
{
    public interface IEmployeeService<Entity, TEntity> where TEntity : class where Entity : class
    {
        IEnumerable<Entity> GetAll();
        Entity Get(int id);
        Task<Entity> Create(TEntity entity);
        Task<Entity> Update(TEntity entity);
        Task<Entity> Delete(int id);
        Task<Entity> Patch(JsonPatchDocument entity, int id);
    }
}
