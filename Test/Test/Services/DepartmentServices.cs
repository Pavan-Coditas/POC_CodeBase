
using EmployeeApiConsumer.Helpers;
using EmployeeApiConsumer.Models;
using System.Net.Http.Headers;

namespace EmployeeApiConsumer.Services
{
    public class DepartmentServices
    {
        readonly HttpClient _client;
        readonly string baseUrl = "https://localhost:7181/";
        readonly JwtHeaderHelper _jwtHelper;
        public DepartmentServices(IHttpClientFactory client, JwtHeaderHelper jwtHeaderHelper)
        {
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
