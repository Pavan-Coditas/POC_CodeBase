using AutoMapper;
using EmployeeManagement.Entities.Models.EntityModels;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Entities.Models.DTOModels;
using EmployeeManagement.Entities.Models.PayloadModel;
using EmployeeManagement.Helper;
using EmployeeManagement.Api.Helper;
using Serilog;
using EmployeeManagement.Api.CustomeMiddlewares;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Domain;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Elasticsearch;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();


//builder.Services.AddDbContext<EmployeeManagementContext>(options =>
//    options.UseSqlServer(configuration.GetConnectionString("SecureConnection")!), ServiceLifetime.Scoped);

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
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddDbContext<EmployeeManagementContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SecureConnection")!);
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
app.UseCors("CORS");
app.UseMiddleware<JwtUserClaimsMiddleware>();
app.UseMiddleware<LogHeaderMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseJwtAuthorization();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.MapControllers();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.Run();
