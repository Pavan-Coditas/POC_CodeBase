using EmployeeApiConsumer.Models;
using EmployeeApiConsumer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Formatting;

namespace EmployeeApiConsumer.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _client;
        private readonly string baseUrl;
        private readonly AccountServices _accountServices;
        private readonly IConfiguration _configuration;


        public AccountController(IHttpClientFactory httpClientFactory, AccountServices accountServices, IConfiguration configuration)
        {
            _configuration = configuration;
            baseUrl = _configuration.GetValue<string>("BaseUrl");
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(baseUrl);
            _accountServices = accountServices;
        }
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> UserLogin(LoginEntity entity)
        {
            try
            {
                var IPAddress = (HttpContext.Items["IpAddress"])!.ToString()!;
                var request = new HttpRequestMessage(HttpMethod.Post, "Login");
                request.Headers.Add("IpAddress", IPAddress);
                request.Content = new ObjectContent<LoginEntity>(entity, new JsonMediaTypeFormatter());
                var result = await _client.SendAsync(request);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseModel = await result.Content.ReadAsAsync<AuthenticationResponse>();
                    HttpContext.Session.SetString("token", responseModel.Token);
                    HttpContext.Session.SetString("UserName", entity.UserName);

                    return RedirectToAction("Navigation", "Comman");
                }
                else
                {
                    TempData["ErrorMsg"] = "Invalid Email or Password";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult RegisterUser()
        {
            return View();
        }
        public async Task<IActionResult> Register(User RegisterModel)
        {
            try
            {
                var result = await _accountServices.RegisterNewUser(RegisterModel);
                if (result)
                {
                    TempData["AlertMessage"] = "Registration Success";
                    return View(result);
                }
                return RedirectToAction("Login");

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult UserLogout()
        {
            try
            {
                HttpContext.Session.Remove("token");
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
