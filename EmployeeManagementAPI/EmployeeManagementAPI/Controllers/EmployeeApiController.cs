namespace EmployeeAPI.Controllers
{
    #region References
    using AutoMapper;
    using EmployeeManagement.Api.Helper.Validators;
    using EmployeeManagement.Entities.Models.DTOModels;
    using EmployeeManagement.Entities.Models.PayloadModel;
    using EmployeeManagment.Services;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    #endregion
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeApiController : ControllerBase
    {

        #region Globals 
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public EmployeeApiController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }
        #endregion

        #region Public Methods
        #region HttpGet
        [Route("/GetAllEmployees")]
        [HttpGet]
        public ActionResult GetAllEmployees()
        {
            var respose = _employeeService.GetAll();
            if (respose != null)
            {
                return Ok(respose);
            }
            return BadRequest();
        }



        [Route("/GetEmployeesById/{id}")]
        [HttpGet]
        public ActionResult GetEmployeesById(int id)
        {
            var respose = _employeeService.Get(id);
            if (respose != null)
            {
                return Ok(respose);
            }
            return BadRequest();
        }

        #endregion

        #region HttpPost
        [Route("/CreateEmployee")]
        [HttpPost]
        [ValidateModel]
        public ActionResult CreateEmployee(Employeepayload employee)
        {
            var employeeDto = _mapper.Map<Employeepayload, EmployeeDTO>(employee);
            var respose = _employeeService.Create(employeeDto);
            if (respose != null)
            {
                return Ok(respose);
            }
            return BadRequest();
        }
        #endregion

        #region HttpDelete
        [Route("/DeleteEmployee")]
        [HttpDelete]
        public ActionResult DeleteEmployee(int id)
        {
            var respose = _employeeService.Delete(id);
            if (respose != null)
            {
                return Ok(respose);
            }
            return BadRequest();
        }

        #endregion

        #region HttpPut
        [Route("/UpdateEmployee")]
        [HttpPut]
        [ValidateModel]
        public ActionResult UpdateEmployee(Employeepayload employee)
        {
            var employeeDto = _mapper.Map<Employeepayload, EmployeeDTO>(employee);

            var respose = _employeeService.Update(employeeDto);
            if (respose != null)
            {
                return Ok(respose);
            }
            return BadRequest();
        }


        #endregion

        #region HttpPatch
        [Route("/PatchEmployee/{id}")]
        [HttpPatch]
        public ActionResult PatchEmployee([FromBody] JsonPatchDocument employeePatch, [FromRoute] int id)
        {
            var response = _employeeService.Patch(employeePatch, id);
            if (response == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        #endregion

        #endregion
    }
}
