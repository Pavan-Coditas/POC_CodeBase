using EmployeeManagment.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : Controller
    {
        #region Globle Varibles
        private readonly IUserService _userService;
        #endregion

        #region Constructors
        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        #region Public Methods
        [Route("/GetallUsers")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var AllUsers = _userService.GetAll().ToList();
            if (AllUsers.Count > 0)
            {
                return Ok(AllUsers);
            }
            return BadRequest();
        }

        #endregion
    }
}

