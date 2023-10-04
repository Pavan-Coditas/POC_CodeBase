using EmployeeManagement.Api.CustomeMiddlewares;
using EmployeeManagement.Api.Helper.Hashing;
using EmployeeManagement.Api.Helper.JWT;
using EmployeeManagement.Api.Helper.RabbitMq;
using EmployeeManagement.Api.Helper.Validators;
using EmployeeManagement.Domain;
using EmployeeManagement.Entities.Models.PayloadModel;
using EmployeeManagement.Services;
using EmployeeManagment.Services;
using EmployeeManagment.Services.Account;
using EmployeeManagment.Services.Cache;
using EmployeeManagment.Services.EmailSending;
using EmployeeManagment.Services.FliterData;
using EmployeeManagment.Services.NCache;
using EmployeeManagment.Services.Services;
using EmployeeManagment.Services.SmsSending;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace EmployeeManagement.Api.Helper.ServiceRegistration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeService, EmployessService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DbContext>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<EmployeeManagementContext>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<IHashingHelper, HashingHelper>();
            services.AddScoped<IJwtTokenGenrator, JwtTokenGenrator>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IValidator<Employeepayload>, EmployeeValidator>();
            services.AddScoped<EmployeeValidator>();
            services.AddControllers();/*.AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);*/
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMvc();
            services.AddLogging();
            services.AddScoped<ExceptionMiddleware>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IPerformanceService, PerformanceService>();
            services.AddScoped<IFilterDataService, FilterDataService>();
            services.AddSignalR();


            services.AddScoped<MsgConfig>();
            services.AddScoped(serviceProvider =>
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri("amqp://guest:guest@localhost:5672"),
                    UserName = "guest",
                    Password = "guest"
                };

                var connection = factory.CreateConnection();
                return connection.CreateModel();
            });

            services.AddSingleton(serviceProvider =>
            {
                var channel = serviceProvider.GetRequiredService<IModel>();
                var queueName = Queue.QueueName;
                return new RabbitMQWorker(channel, queueName);
            });

            var worker = services.BuildServiceProvider().GetRequiredService<RabbitMQWorker>();
            worker.StartListening();

            services.AddCors(options =>
            {
                options.AddPolicy("CORS", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            return services;
        }
    }
}
