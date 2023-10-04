using EmployeeManagment.Services.FliterData;
using EmployeeManagment.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterDataController : ControllerBase
    {
        private readonly IFilterDataService _filterDataService;
        public FilterDataController(IFilterDataService filterDataService)
        {
            _filterDataService = filterDataService;
        }
        [Route("/GetFilterdData")]
        [HttpPost]
        public IActionResult Get(string spName, SPPararm? parameters = null)
        {
            var result = _filterDataService.GetFilterData(spName, parameters);
            return Ok(result);
        }
    }
}
