using EmployeeManagement.Api.CustomeMiddlewares;
using EmployeeManagement.Api.Helper.Validators;
using EmployeeManagement.Domain;
using EmployeeManagement.Entities.Models.DTOModels;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagement.Entities.Models.PayloadModel;
using EmployeeManagement.Services;
using EmployeeManagment.Services;
using EmployeeManagment.Services.Account;
using EmployeeManagment.Services.Cache;
using EmployeeManagment.Services.EmailSending;
using EmployeeManagment.Services.Services;
using EmployeeManagment.Services.SmsSending;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace EmployeeManagement.Api.Helper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeService<Employee, EmployeeDTO>, EmployessService>();
            services.AddScoped<IDepartmentService<Department>, DepartmentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<UnitOfWorkFactory>();
            services.AddScoped<DbContext>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<HashingHelper>();
            services.AddScoped<JwtTokenGenrator>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IValidator<Employeepayload>, EmployeeValidator>();
            services.AddScoped<EmployeeValidator>();
            services.AddControllers().AddNewtonsoftJson();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMvc();
            services.AddLogging();
            services.AddScoped<ExceptionMiddleware>();
            services.AddScoped<IAccountService,AccountService>();
            services.AddTransient<IEmailSender, EmailSender>();
            return services;
        }
    }
}
