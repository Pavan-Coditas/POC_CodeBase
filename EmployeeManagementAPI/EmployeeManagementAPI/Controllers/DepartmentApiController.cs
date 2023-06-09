﻿
namespace EmployeeAPI.Controllers
{

    #region References
    using EmployeeManagement.Entities.Models.EntityModels;
    using EmployeeManagment.Services;
	using Microsoft.AspNetCore.Mvc;
    #endregion

    #region Department Controller

    #region Routes
    [Route("api/[controller]")]
    [ApiController]
	#endregion
	public class DepartmentApiController : ControllerBase
    {
        #region Globle Varibles
        private readonly IDepartmentService<Department> _departmentService;
        #endregion

        #region Constructors
        public DepartmentApiController(IDepartmentService<Department> departmentService)
        {
            _departmentService = departmentService;
        }
        #endregion

        #region Public Methods
        [Route("/Getalldepartments")]
        [HttpGet]
        public IActionResult GetAllDepartments()
        {
            var AllDepartment = _departmentService.GetAll().ToList();
            if (AllDepartment.Count > 0)
            {
                return Ok(AllDepartment);
            }
           return BadRequest();
        }
        #endregion

    }
    #endregion

}
