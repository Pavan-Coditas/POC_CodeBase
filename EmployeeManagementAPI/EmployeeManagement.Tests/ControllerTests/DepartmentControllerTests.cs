using EmployeeAPI.Controllers;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployeeManagement.NunitTests.ControllerTests
{
    public class DepartmentControllerTests
    {
        private Mock<IDepartmentService> _mockService;
        private DepartmentApiController _mockApiController;
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IDepartmentService>();
            _mockApiController = new DepartmentApiController(_mockService.Object);
        }

        [Test]
        public void GetAllDepartments_ReturnsAllDepartments_WhenDepartments()
        {
            //Arrange
            var departments = new List<Department>()
            {
                new Department(){ Id=1000,Name="Development"},
                new Department(){ Id=1001,Name="QA"},
                new Department(){ Id=1002,Name="HR"},
            };
            _mockService.Setup(x => x.GetAll()).Returns(departments);
            //Act
            var depts = _mockApiController.GetAllDepartments();

            //Assert
            Assert.That(depts, Is.InstanceOf<OkObjectResult>());
            var result = (OkObjectResult)depts;
            var alldpts = result.Value;
            Assert.IsNotNull(alldpts, "Token should be a non-null string");
        }

        [Test]
        public void GetAllDepartments_ReturnsbadRequest_WhenDepartmentsNotFound()
        {
            //Arrange
            var departments = new List<Department>();
            _mockService.Setup(x => x.GetAll()).Returns(departments);
            //Act
            var depts = _mockApiController.GetAllDepartments();

            //Assert
            Assert.That(depts, Is.InstanceOf<BadRequestResult>());
        }

    }
}
