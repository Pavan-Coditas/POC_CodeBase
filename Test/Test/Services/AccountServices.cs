using EmployeeApiConsumer.Helpers;
using EmployeeApiConsumer.Models;

namespace EmployeeApiConsumer.Services
{
    public class AccountServices
    {

        private readonly HttpClient _client;
        private readonly string baseUrl;
        private readonly JwtHeaderHelper _jwtHelper;
        private readonly IConfiguration _configuration;

        public AccountServices(HttpClient client, JwtHeaderHelper jwtHeaderHelper, IConfiguration configuration)
        {
            _configuration = configuration;
            baseUrl = _configuration.GetValue<string>("BaseUrl");
            _client = client;
            _client.BaseAddress = new Uri(baseUrl);
            _jwtHelper = jwtHeaderHelper;
            _jwtHelper.AddJwtToHeaders(_client.DefaultRequestHeaders);
        }

        public async Task<bool> RegisterNewUser(User user)
        {
            user.CreatedBy = "Self";
            user.ModifiedBy = "Self";
            user.CreatedOn = DateTime.Now;
            user.ModifiedOn = DateTime.Now;
            user.GenderId = 10;
            user.Id = Guid.NewGuid().ToString();
            var result = await _client.PostAsJsonAsync($"Register", user);
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
