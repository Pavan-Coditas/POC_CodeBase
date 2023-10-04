using EmployeeManagment.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagment.Services.FliterData
{
    public interface IFilterDataService
    {
        FilteredDataResult GetFilterData(string spName, SPPararm? parameters = null);
    }
}
