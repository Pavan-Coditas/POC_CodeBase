using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagment.Services
{
    public interface IDepartmentService<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
    }
}
