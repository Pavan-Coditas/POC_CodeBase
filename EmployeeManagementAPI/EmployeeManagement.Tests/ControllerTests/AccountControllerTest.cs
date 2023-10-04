using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Api.Helper.Hashing;
using EmployeeManagement.Api.Helper.JWT;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services.Account;
using EmployeeManagment.Services.EmailSending;
using EmployeeManagment.Services.SmsSending;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EmployeeManagement.NunitTests.ControllerTests
{
    public class AccountControllerTests
    {
        private Mock<IAccountService> _accountServiceMock;
        private Mock<IJwtTokenGenrator> _tokenGenratorMock;
        private Mock<IHashingHelper> _hashMock;
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<ISmsSender> _smsSenderMock;
        private AccountController _accountController;

        [SetUp]
        public void Setup()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _tokenGenratorMock = new Mock<IJwtTokenGenrator>();
            _hashMock = new Mock<IHashingHelper>();
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

        [Test]
        public void Login_ReturnsUnAuthorized_WhenPasswordIsWrong()
        {
            // Arrange
            var entity = new LoginModel()
            {
                UserName = "Pavan",
                Password = "wrongpassword"
            };

            var expectedresult = new User()
            {
                UserName = "Pavan",
                Password = "d5b7939838bb780764bdf82e55c6ec40ce206981f11e5c5627b65e8a8dc57738"
            };
            _accountServiceMock.Setup(x => x.GetUser(entity)).Returns(expectedresult);
            // Act
            var result = _accountController.Login(entity);

            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public void Login_ReturnsJwtToken_WhenUserAuthorized()
        {
            //Arrange
            var entity = new LoginModel()
            {
                Password = "Coditas124@",
                UserName = "Pavan"
            };
            var expectedResult = new User()
            {
                UserName = "Pavan",
                Password = "d5b7939838bb780764bdf82e55c6ec40ce206981f11e5c5627b65e8a8dc57738"
            };
            _accountServiceMock.Setup(x => x.GetUser(entity)).Returns(expectedResult);

            var expectedToken = "mocked_token";
            var jwtSettings = new Dictionary<string, string>
            {
                { "SecretKey", "your_secret_key" },
                { "Issuer", "your_issuer" },
                { "Audience", "your_audience" },
                { "ExpirationInMinutes", "30" }
            };

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection("JwtSettings")).Returns(new ConfigurationSection(new ConfigurationRoot(new List<IConfigurationProvider>()), "JwtSettings"));
            configurationMock.Setup(x => x["JwtSettings:SecretKey"]).Returns(jwtSettings["SecretKey"]);
            configurationMock.Setup(x => x["JwtSettings:Issuer"]).Returns(jwtSettings["Issuer"]);
            configurationMock.Setup(x => x["JwtSettings:Audience"]).Returns(jwtSettings["Audience"]);
            configurationMock.Setup(x => x["JwtSettings:ExpirationInMinutes"]).Returns(jwtSettings["ExpirationInMinutes"]);

            _tokenGenratorMock.Setup(x => x.GenerateJSONWebToken(entity)).Returns(new { Token = expectedToken });
            _hashMock.Setup(x => x.VerifyHash(expectedResult.Password, entity.Password)).Returns(true);
            //Act
            var result = _accountController.Login(entity);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = (OkObjectResult)result;
            var token = okResult.Value!.ToString();
            Assert.IsNotNull(token, "Token should be a non-null string");
        }

        [Test]
        public void Register_ReturnsOkResult_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var user = new User
            {
                UserName = "testuser",
                Password = "password123"
            };

            var hashedPassword = "hashed_password";
            _hashMock.Setup(x => x.OneWayHash(user.Password)).Returns(hashedPassword);

            var expectedResult = new User
            {
                UserName = user.UserName,
                Password = hashedPassword
                // Set other properties as needed
            };
            _accountServiceMock.Setup(x => x.CreateUser(user)).Returns(expectedResult);

            // Act
            var result = _accountController.Register(user);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Register_ReturnsBadRequest_WhenUserNotCreatedSuccessfully()
        {
            // Arrange
            var user = new User
            {
                UserName = "testuser",
                Password = "password123"
            };

            var hashedPassword = "hashed_password";
            _hashMock.Setup(x => x.OneWayHash(user.Password)).Returns(hashedPassword);

            var expectedResult = new User
            {
                UserName = user.UserName,
                Password = hashedPassword
                // Set other properties as needed
            };
            _accountServiceMock.Setup(x => x.CreateUser(user)).Returns((User)null!);

            // Act
            var result = _accountController.Register(user);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        //[Test]
        //public async Task SendEmail_WhenCalled_ShouldSendEmail()
        //{
        //    Arrange
        //   var email = "test@example.com";
        //    var subject = "Test Subject";
        //    var message = "Test Message";
        //    var emailSenderMock = new Mock<IEmailSender>();
        //    Act
        //   await _accountController.SendEmail(email, subject, message);

        //    Assert
        //    emailSenderMock.Verify(x => x.SendEmailAsync(email, subject, message), Times.Once);
        //}

        //[Test]
        //public void SmsSender_ShouldReturnOkResult()
        //{
        //    //Arrange
        //    var mockSmsSender = new Mock<ISmsSender>();
        //    mockSmsSender.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
        //    //Act
        //    var result = _accountController.SmsSender("1234567890", "Hello World!");

        //    //Assert
        //    Assert.IsInstanceOf<OkObjectResult>(result);
        //    Assert.That((result as OkObjectResult)!.Value, Is.EqualTo("OK"));
        //}
    }
}