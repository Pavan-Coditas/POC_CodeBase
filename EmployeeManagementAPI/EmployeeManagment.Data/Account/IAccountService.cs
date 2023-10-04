using EmployeeManagement.Entities.Models.EntityModels;

namespace EmployeeManagment.Services.Account
{
    public interface IAccountService
    {
        User GetUser(LoginModel model);
        void LoginAudit(LoginAudit audit);
        User CreateUser(User user);
    }
}
