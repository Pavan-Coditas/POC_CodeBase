using EmployeeApiConsumer.Models;
using EmployeeApiConsumer.Services.FilterData;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApiConsumer.Controllers
{
    public class FilterDataController : Controller
    {
        private readonly FilterDataService _filterDataService;
        public FilterDataController(FilterDataService filterDataService)
        {
            _filterDataService = filterDataService;
        }

        public IActionResult GetSechduleReportDashboard()
        {
            return View();
        }

        public async Task<IActionResult> GetFilterData(SPPararm Pararms=null!)
        {
            var res=await _filterDataService.GetFilterData(Pararms);
            return Json(res);
        }
    }
}
