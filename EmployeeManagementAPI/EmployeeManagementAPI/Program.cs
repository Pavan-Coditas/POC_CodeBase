using AutoMapper;
using EmployeeManagement.Api.CustomeMiddlewares;
using EmployeeManagement.Api.Helper.AutoMapper;
using EmployeeManagement.Api.Helper.ServiceRegistration;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Domain;
using EmployeeManagement.Entities.Models.DTOModels;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagement.Entities.Models.PayloadModel;
using EmployeeMangementApi.Hub.Hub;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;


var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();


//builder.Services.AddScoped<ICache>(provider =>
//{
//    var cache = CacheManager.GetCache(configuration["NCacheSettings:CacheID"]);
//    return cache;
//});
//builder.Services.AddScoped<INCacheService, NCacheService>();

Log.Logger = new LoggerConfiguration()
    .Enrich.WithProcessId()
    .Enrich.WithCorrelationId()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

Log.Logger = new LoggerConfiguration()
    .Enrich.WithProcessId()
    .Enrich.WithCorrelationId()
    .Enrich.FromLogContext()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://pavansdep.es.us-central1.gcp.cloud.es.io"))
    {
        IndexFormat = "search-my-application-{0:yyyy.MM}",
        AutoRegisterTemplate = true,
        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
        ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "bzCjPgbJ8XLSFNISUyGxmnbb"),
        EmitEventFailure =
                EmitEventFailureHandling.WriteToSelfLog |
                EmitEventFailureHandling.RaiseCallback |
                EmitEventFailureHandling.ThrowException,
        FailureCallback = (e) =>
        {
            Console.WriteLine("Unable to submit event " + e.MessageTemplate);
        }
    })
    .CreateLogger();

builder.Services.AddDbContext<EmployeeManagementContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SecureConnection")!);
});

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", builder => builder.WithOrigins("https://localhost:7088").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

var mappingConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(typeof(MyMappingHelper<,>).MakeGenericType(typeof(Employeepayload), typeof(EmployeeDTO)));
    cfg.AddProfile(typeof(MyMappingHelper<,>).MakeGenericType(typeof(EmployeeDTO), typeof(Employee)));
});
IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.RegisterServices();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CORSPolicy");
app.UseMiddleware<JwtUserClaimsMiddleware>();
app.UseMiddleware<LogHeaderMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
//app.UseJwtAuthorization();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.MapControllers();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
