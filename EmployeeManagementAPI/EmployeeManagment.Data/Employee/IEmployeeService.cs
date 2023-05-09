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
        Entity Create(TEntity entity);
        Entity Update(TEntity entity);
        Entity Delete(int id);
        Entity Patch(JsonPatchDocument entity, int id);
    }
}
