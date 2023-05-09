using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace EmployeeApiConsumer.Entities.Models.EntityModels
{
    public class LoginModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; }= string.Empty;
    }
}
