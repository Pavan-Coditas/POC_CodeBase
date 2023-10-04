using EmployeeManagement.Domain.Models.ResponseModel;
using EmployeeManagment.Services.NCache;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceController : Controller
    {
        private readonly IPerformanceService _nCachePerformance;
        public PerformanceController(IPerformanceService nCachePerformance)
        {
            _nCachePerformance = nCachePerformance;
        }
        //[Route("/UsingNCache")]
        //[HttpGet]
        //public ActionResult UsingNCache()
        //{
        //    var respose = _nCachePerformance.GetAllUsingNcash();
        //    if (respose != null)
        //    {
        //        return Ok(respose);
        //    }
        //    return BadRequest();
        //}
        [Route("/UsingRedis")]
        [HttpGet]
        public ActionResult UsingRedis()
        {
            var respose = _nCachePerformance.GetUsingRedis();
            if (respose != null)
            {
                return Ok(respose);
            }
            return BadRequest();
        }

        [Route("/JsonTestGetData")]
        [HttpGet]
        public ActionResult GetPersons()
        {
            var res = new PerformanceResponse();
            var timeTaken = Stopwatch.StartNew();
            //var respose = _nCachePerformance.GetPersons();
            //await _nCachePerformance.DeserializeAndBulkInsert(respose);
            timeTaken.Stop();

            res.Message = "Time Taken To process 10000 records";
            res.TimeTaken = timeTaken.ElapsedMilliseconds.ToString() + " Milliseconds"!;

            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest();
        }

        [Route("/RuleEnginePeroformance")]
        [HttpGet]
        public ActionResult GetPerson()
        {
            var res = new PerformanceResponse();
            var timeTaken = Stopwatch.StartNew();
            var employees = _nCachePerformance.GetAllPersons();
            timeTaken.Stop();

            res.Message = "Time Taken To process 10000 records";
            res.TimeTaken = timeTaken.ElapsedMilliseconds.ToString() + " Milliseconds"!;

            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest();
        }

    }
}
