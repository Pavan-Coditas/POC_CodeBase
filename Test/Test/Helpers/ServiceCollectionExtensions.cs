using EmployeeApiConsumer.Services;

namespace EmployeeApiConsumer.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddScoped<EmployeeServices>();
            services.AddScoped<DepartmentServices>();
            services.AddScoped<JwtHeaderHelper>();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddMvc();
            return services;
        }
    }
}
