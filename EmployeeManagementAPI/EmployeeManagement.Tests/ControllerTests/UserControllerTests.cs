using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployeeManagement.NunitTests.ControllerTests
{
    public class UserControllerTests
    {
        private Mock<IUserService> _mockUserService;
        private UserApiController _userService;
        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>();
            _userService = new UserApiController(_mockUserService.Object);

        }
        [Test]
        public void GetAllUsers_ShouldReturnOkResult()
        {
            // Arrange
            _mockUserService.Setup(repo => repo.GetAll()).Returns(new List<User> { new User { UserName = "john cena", Password = "wwe" } });

            // Act
            var result = _userService.GetAllUsers();

            // Assert

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void GetAllUsers_ShouldReturnBadRequestResult()
        {
            // Arrange
            _mockUserService.Setup(repo => repo.GetAll()).Returns(new List<User>());

            // Act
            var result = _userService.GetAllUsers();

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }
    }
}
