using AutoMapper;
using EmployeeAPI.Controllers;
using EmployeeManagement.Entities.Models.DTOModels;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagement.Entities.Models.PayloadModel;
using EmployeeManagment.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployeeManagement.NunitTests.ControllerTests
{
    public class EmployeeControllerTests
    {
        private Mock<IEmployeeService> _mockEmployeeService;
        private Mock<IMapper> _mockMapper;
        private EmployeeApiController _mockEmployeeApiController;
        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockEmployeeService = new Mock<IEmployeeService>();
            _mockEmployeeApiController = new EmployeeApiController(_mockEmployeeService.Object, _mockMapper.Object);
        }

        [Test]
        public void GetAllEmployees_ShouldReturnOkResult()
        {
            // Arrange
            _mockEmployeeService.Setup(x => x.GetAll()).Returns(new List<Employee>());
            var controller = new EmployeeApiController(_mockEmployeeService.Object, _mockMapper.Object);

            // Act
            var result = _mockEmployeeApiController.GetAllEmployees();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetAllEmployees_ShouldReturnBadRequestResult()
        {
            // Arrange
            _mockEmployeeService.Setup(x => x.GetAll()).Returns((List<Employee>)null!);

            // Act
            var result = _mockEmployeeApiController.GetAllEmployees();

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void GetEmployeesById_ShouldReturnOkResult_WhenEmployeeExists()
        {
            // Arrange
            _mockEmployeeService.Setup(x => x.Get(It.IsAny<int>())).Returns(new Employee());


            // Act
            var result = _mockEmployeeApiController.GetEmployeesById(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetEmployeesById_ShouldReturnBadRequestResult_WhenEmployeeDoesNotExist()
        {
            // Arrange
            _mockEmployeeService.Setup(x => x.Get(It.IsAny<int>())).Returns((Employee)null!);

            // Act
            var result = _mockEmployeeApiController.GetEmployeesById(1);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void CreateEmployee_ShouldReturnOkResult()
        {
            // Arrange
            var employeepayload = new Employeepayload { Id = 1, Name = "John Doe" };
            var employeeDto = new EmployeeDTO { Id = 1, Name = "John Doe" };
            var employee = new Employee { Id = 1, Name = "John Doe" };

            _mockMapper.Setup(x => x.Map<Employeepayload, EmployeeDTO>(employeepayload)).Returns(employeeDto);
            _mockEmployeeService.Setup(x => x.Create(employeeDto)).Returns(employee);

            // Act
            var result = _mockEmployeeApiController.CreateEmployee(employeepayload);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void CreateEmployee_ShouldReturnBadRequestResult()
        {
            // Arrange
            var employee = new Employeepayload { Id = 1, Name = "John Doe" };
            var employeeDto = new EmployeeDTO { Id = 1, Name = "John Doe" };
            _mockMapper.Setup(x => x.Map<Employeepayload, EmployeeDTO>(employee)).Returns(employeeDto);
            _mockEmployeeService.Setup(x => x.Create(employeeDto)).Returns((Employee)null!);

            // Act
            var result = _mockEmployeeApiController.CreateEmployee(employee);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void DeleteEmployee_ShouldReturnOkResult_WhenEmployeeExists()
        {

            // Arrange
            var employee = new Employee { Id = 1, Name = "John Doe" };
            _mockEmployeeService.Setup(x => x.Delete(It.IsAny<int>())).Returns(employee);


            // Act
            var result = _mockEmployeeApiController.DeleteEmployee(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void DeleteEmployee_ShouldReturnBadRequestResult_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<IEmployeeService>();
            mockService.Setup(x => x.Delete(It.IsAny<int>())).Returns((Employee)null!);


            // Act
            var result = _mockEmployeeApiController.DeleteEmployee(1);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void UpdateEmployee_ShouldReturnOkResult()
        {
            // Arrange
            var employeePayload = new Employeepayload { Id = 1, Name = "John Doe" };
            var employeeDto = new EmployeeDTO { Id = 1, Name = "John Doe" };
            var employee = new Employee { Id = 1, Name = "John Doe" };
            _mockMapper.Setup(x => x.Map<Employeepayload, EmployeeDTO>(employeePayload)).Returns(employeeDto);
            _mockEmployeeService.Setup(x => x.Update(employeeDto)).Returns(employee);

            // Act
            var result = _mockEmployeeApiController.UpdateEmployee(employeePayload);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void UpdateEmployee_ShouldReturnBadRequestResult()
        {
            // Arrange
            var employee = new Employeepayload { Id = 1, Name = "John Doe" };
            var employeeDto = new EmployeeDTO { Id = 1, Name = "John Doe" };
            _mockMapper.Setup(x => x.Map<Employeepayload, EmployeeDTO>(employee)).Returns(employeeDto);
            _mockEmployeeService.Setup(x => x.Update(employeeDto)).Returns((Employee)null!);

            // Act
            var result = _mockEmployeeApiController.UpdateEmployee(employee);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void PatchEmployee_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            _mockEmployeeService.Setup(repo => repo.Patch(It.IsAny<JsonPatchDocument>(), It.IsAny<int>())).Returns(new Employee());

            // Act
            var result = _mockEmployeeApiController.PatchEmployee(new JsonPatchDocument(), 1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void PatchEmployee_WhenCalled_ReturnsNotFoundResult()
        {
            // Arrange
            _mockEmployeeService.Setup(repo => repo.Patch(It.IsAny<JsonPatchDocument>(), It.IsAny<int>())).Returns((Employee)null!);
            var controller = new EmployeeApiController(_mockEmployeeService.Object, _mockMapper.Object);

            // Act
            var result = controller.PatchEmployee(new JsonPatchDocument(), 1);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
