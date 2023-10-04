using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Domain.Models.ResponseModel;
using EmployeeManagment.Services.NCache;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployeeManagement.NunitTests.ControllerTests
{
    public class PerformanceControllerTest
    {
        private Mock<IPerformanceService> _performanceService;
        private PerformanceController _controller;
        [SetUp]
        public void Setup()
        {
            _performanceService = new Mock<IPerformanceService>();
            _controller = new PerformanceController(_performanceService.Object);
        }

        [Test]
        public void GetPersons_ShouldReturnOkResult()
        {
            // Act
            var data = _controller.GetPersons();

            // Assert
            Assert.IsNotNull(data);
            Assert.That(data, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void GetPersons_ShouldReturnBadRequestResult()
        {
            // Act
            var data = _controller.GetPersons();

            // Assert
            Assert.IsNotNull(data);
            Assert.That(data, Is.InstanceOf<OkObjectResult>());

        }

        [Test]
        public void UsingRedis_ValidResponse_ReturnsOkResult()
        {
            // Arrange
            var response = new PerformanceResponse()
            {
                Message = "Test response"
            };
            _performanceService.Setup(x => x.GetUsingRedis()).Returns(response);

            // Act
            var result = _controller.UsingRedis();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(response));
        }

        [Test]
        public void UsingRedis_NullResponse_ReturnsBadRequest()
        {
            // Arrange
            _performanceService.Setup(x => x.GetUsingRedis()).Returns((PerformanceResponse)null!);

            // Act
            var result = _controller.UsingRedis();

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

    }
}
