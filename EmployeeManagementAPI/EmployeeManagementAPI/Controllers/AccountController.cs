using EmployeeManagement.Api.Helper.Hashing;
using EmployeeManagement.Api.Helper.JWT;
using EmployeeManagement.Context;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services.Account;
using EmployeeManagment.Services.EmailSending;
using EmployeeManagment.Services.SmsSending;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtTokenGenrator _tokenGenrator;
        private readonly IHashingHelper _hash;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        // Constructor to inject required services
        public AccountController(IHashingHelper hash, IJwtTokenGenrator tokenGenrator, IAccountService dbAccountService, IEmailSender emailSender, ISmsSender smsSender)
        {
            _hash = hash;
            _tokenGenrator = tokenGenrator;
            _accountService = dbAccountService;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        // Route to handle login request
        [Route("/Login")]
        [HttpPost]
        public IActionResult Login(LoginModel entity)
        {
            // Create a login audit object to track user activity
            var audit = new LoginAudit
            {
                AudiType = "Login",
                CreatedOn = DateTime.Now,
                Ipaddress = UserContext.IpAddress
            };

            // Call the GetUser method of account service to retrieve user details
            var user = _accountService.GetUser(entity);

            // If the user is not found, return a bad request response
            if (user == null)
            {
                audit.UserName = entity.UserName;
                audit.AuditStatus = "Login Failed";
                _accountService.LoginAudit(audit);
                return BadRequest();
            }

            // Verify user password using VerifyHash method of HashingHelper
            if (!_hash.VerifyHash(user.Password, entity.Password))
            {
                audit.UserName = entity.UserName;
                audit.AuditStatus = "Password is Incorrect";
                _accountService.LoginAudit(audit);
                return Unauthorized();
            }

            // If the user is authenticated, create a login audit and return a JWT token
            audit.UserName = user.UserName;
            audit.AuditStatus = "Login Success";
            _accountService.LoginAudit(audit);
            var response = _tokenGenrator.GenerateJSONWebToken(entity);
            return Ok(response);
        }

        // Route to handle user registration request
        [Route("/Register")]
        [HttpPost]
        public IActionResult Register(User user)
        {
            // Hash the user password using OneWayHash method of HashingHelper
            user.Password = _hash.OneWayHash(user.Password);

            // Call the CreateUser method of account service to create a new user
            var res = _accountService.CreateUser(user);

            // If the user is successfully created, return an OK response with the user details
            if (res != null)
            {
                return Ok(res);
            }

            // Otherwise, return a bad request response
            return BadRequest();
        }

        [HttpPost("/SendEmail")]
        public async Task<IActionResult> SendEmail(string email, string subject, string message)
        {
            await _emailSender.SendEmailAsync(email, subject, message);
            return Ok();
        }


        [HttpPost("/SendSms")]
        public IActionResult SmsSender(string recipentNumber, string message)
        {
            var ack = _smsSender.SendSms(recipentNumber, message);
            return Ok(ack);
        }

    }
}
