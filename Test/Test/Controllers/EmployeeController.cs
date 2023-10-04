using EmployeeApiConsumer.Models;
using EmployeeApiConsumer.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{

    public class EmployeeController : Controller
    {
        readonly EmployeeServices _employeeServices;
        readonly DepartmentServices _departmentServices;

        public EmployeeController(EmployeeServices employeeServices, DepartmentServices departmentServices)
        {
            _employeeServices = employeeServices;
            _departmentServices = departmentServices;
        }

        public ActionResult EmployeeDashboard()
        {
            return View();
        }

        public async Task<JsonResult> GetAllDepartments()
        {
            var departments = await _departmentServices.GetDepartmentsAsync();
            return Json(departments);
        }


        public async Task<ActionResult> GetAllEmployees()
        {
            ViewBag.departments = await _departmentServices.GetDepartmentsAsync();
            var employees = await _employeeServices.GetEmployeesAsync();
            //return PartialView("_EmployeeGrid", employees);
            return Json(employees);
        }

        public async Task<ActionResult> Create()
        {
            ViewBag.departments = await _departmentServices.GetDepartmentsAsync();
            return PartialView("CreateNew");
        }

        public async Task<JsonResult> CreateEmployee(Employee employee)
        {
            var result = await _employeeServices.AddEmployeeAsync(employee);
            return Json(employee);
        }


        public async Task<ActionResult> Edit(int id)
        {

            ViewBag.departments = await _departmentServices.GetDepartmentsAsync();
            var emp = await _employeeServices.GetEmployeeByIdAsync(id);
            return Json(emp);
        }

        [HttpPost]
        public async Task<ActionResult> EditEmp(Employee employee)
        {
            var respose = await _employeeServices.EditEmployeeAsync(employee);
            return Json(employee);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var respose = await _employeeServices.DeletEmployeeAsync(id);
            return Json("Record Deleted Successfully");
        }

        public async Task<ActionResult> PatchEmployee([FromBody] JsonPatchDocument patchDocument, int id)
        {
            var response = await _employeeServices.PatchEmployee(patchDocument, id);
            return Json("Record Updated Successfully");
        }
    }
}