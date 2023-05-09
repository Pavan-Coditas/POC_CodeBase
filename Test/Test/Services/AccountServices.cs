using EmployeeApiConsumer.Helpers;
using EmployeeApiConsumer.Models;
using System.Net.Http.Headers;

namespace EmployeeApiConsumer.Services
{
    public class AccountServices
    {

        readonly HttpClient _client;
        readonly string baseUrl = "https://localhost:7181/";
        readonly JwtHeaderHelper _jwtHelper;
        public AccountServices(HttpClient client, JwtHeaderHelper jwtHeaderHelper)
        {
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
            var result = await _client.PostAsJsonAsync($"Register", RegisterNewUser);
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
