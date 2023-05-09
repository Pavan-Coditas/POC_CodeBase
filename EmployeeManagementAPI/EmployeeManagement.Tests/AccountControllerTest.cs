using DryIoc;
using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Api.Helper;
using EmployeeApiConsumer.Entities.Models.EntityModels;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services.Account;
using EmployeeManagment.Services.EmailSending;
using EmployeeManagment.Services.SmsSending;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Castle.Core.Configuration;

namespace EmployeeManagement.Tests
{
    public class AccountControllerTests
    {
        private Mock<IAccountService> _accountServiceMock;
        private Mock<JwtTokenGenrator> _tokenGenratorMock;
        private Mock<HashingHelper> _hashMock;
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<ISmsSender> _smsSenderMock;
        private AccountController _accountController;

        [SetUp]
        public void Setup()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _tokenGenratorMock = new Mock<JwtTokenGenrator>();
            _hashMock = new Mock<HashingHelper>();
            _emailSenderMock = new Mock<IEmailSender>();
            _smsSenderMock = new Mock<ISmsSender>();
            _accountController = new AccountController(
                _hashMock.Object,
                _tokenGenratorMock.Object,
                _accountServiceMock.Object,
                _emailSenderMock.Object,
                _smsSenderMock.Object
            );
        }

        [Test]
        public void Login_ReturnsBadRequest_WhenUserIsNotFound()
        {
            // Arrange
            var entity = new LoginModel();
            _accountServiceMock.Setup(x => x.GetUser(entity)).Returns((User)null!);

            // Act
            var result = _accountController.Login(entity);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }
    }
}