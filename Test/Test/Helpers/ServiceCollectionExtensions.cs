using ConnectAndSell.Common;
using EmployeeApiConsumer.Services;
using EmployeeConsumer.Services;

namespace EmployeeApiConsumer.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddScoped<EmployeeServices>();
            services.AddScoped<DepartmentServices>();
            services.AddScoped<AccountServices>();
            services.AddScoped<ChatService>();
            services.AddScoped<HttpClient>();
            services.AddScoped<JwtHeaderHelper>();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddSignalR();
            services.AddScoped<HttpHelper>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            return services;
        }
    }
}
