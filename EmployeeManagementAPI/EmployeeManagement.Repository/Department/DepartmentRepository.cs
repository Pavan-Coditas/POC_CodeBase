using EmployeeManagement.Domain;
using EmployeeManagement.Entities.Models;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagement.Repository.Generic;
using Microsoft.AspNetCore.JsonPatch;

namespace EmployeeManagement.Repository
{
    public class DepartmentRepository : IGenericRepository<Department,int>
    {
        EmployeeManagementContext _context;
        public DepartmentRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This methods gets all the departments
        /// </summary>
        /// <returns>list of dept</returns>
        public IEnumerable<Department> GetAll()
        {
            return _context.Departments.ToList();
        }

        public Department Create(Department entity)
        {
            throw new NotImplementedException();
        }

        public Department Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Department Get(int id)
        {
            throw new NotImplementedException();
        }

        public Department Patch(int id, JsonPatchDocument entity)
        {
            throw new NotImplementedException();
        }

        public Department Update(Department entity)
        {
            throw new NotImplementedException();
        }
    }
}
