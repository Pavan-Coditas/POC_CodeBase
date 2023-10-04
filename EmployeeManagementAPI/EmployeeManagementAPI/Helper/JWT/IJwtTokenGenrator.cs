using EmployeeManagement.Entities.Models.EntityModels;

namespace EmployeeManagement.Api.Helper.JWT
{
    public interface IJwtTokenGenrator
    {
        object GenerateJSONWebToken(LoginModel entity);
    }
}
