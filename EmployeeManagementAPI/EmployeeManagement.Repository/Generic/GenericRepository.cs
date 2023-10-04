using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repository.Generic
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<Entity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<Entity>();
        }

        public Entity Create(Entity entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public Entity Delete(int id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            return entity;
        }

        public List<Entity> GetAll()
        {
            return _dbSet.ToList();
        }

        public Entity Get(int id)
        {
            return _dbSet.Find(id)!;
        }

        public Entity Update(Entity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public Entity Patch(int id, JsonPatchDocument entity)
        {
            var entityToUpdate = Get(id);

            entity.ApplyTo(entityToUpdate);

            return Update(entityToUpdate);
        }
    }
}
