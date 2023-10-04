using Microsoft.AspNetCore.JsonPatch;

namespace EmployeeManagement.Repository.Generic
{
    public interface IGenericRepository<Entity> where Entity : class
    {
        Entity Create(Entity entity);
        Entity Delete(int id);
        List<Entity> GetAll();
        Entity Get(int id);
        Entity Update(Entity entity);
        Entity Patch(int id, JsonPatchDocument entity);
    }
}
