using EmployeeApiConsumer.CustomeMiddlewares;
using EmployeeApiConsumer.Helpers;
using EmployeeApiConsumer.Middleware;
using EmployeeApiConsumer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddScoped<EmployeeServices>();
builder.Services.AddScoped<DepartmentServices>();
builder.Services.AddScoped<AccountServices>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<JwtHeaderHelper>();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddMvc();

builder.Services.RegisterServices();


//builder.Services.AddControllersWithViews(config => config.Filters.Add(typeof(CustomExceptionFilter)));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error");
    
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithRedirects("/Comman/Error?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<IpAddressMiddleware>();
app.UseMiddleware<CorelationIdMiddleware>();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
