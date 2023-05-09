﻿using EmployeeApiConsumer.Entities.Models.EntityModels;
using EmployeeManagement.Entities.Models.EntityModels;

namespace EmployeeManagment.Services.Account
{
    public interface IAccountService
    {
        User GetUser(LoginModel model);
        Task LoginAudit(LoginAudit audit);
        User CreateUser(User user);
    }
}
