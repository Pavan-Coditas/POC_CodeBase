
using EmployeeApiConsumer.Helpers;
using EmployeeApiConsumer.Models;

namespace EmployeeApiConsumer.Services
{
    public class DepartmentServices
    {
        private readonly HttpClient _client;
        private readonly string baseUrl;
        private readonly JwtHeaderHelper _jwtHelper;
        private readonly IConfiguration _configuration;
        public DepartmentServices(IHttpClientFactory client, JwtHeaderHelper jwtHeaderHelper, IConfiguration configuration)
        {
            _configuration = configuration;
            baseUrl = _configuration.GetValue<string>("BaseUrl");
            _client = client.CreateClient();
            _jwtHelper = jwtHeaderHelper;
            _jwtHelper.AddJwtToHeaders(_client.DefaultRequestHeaders);
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            var departments = await _client.GetFromJsonAsync<List<Department>>($"{baseUrl}Getalldepartments");
            if (departments == null)
            {
                throw new Exception("There are no Departments");
            }
            return departments;
        }

    }
}
