using EmployeeApiConsumer.Helpers;
using EmployeeApiConsumer.Models;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using System.Text;

namespace EmployeeApiConsumer.Services
{
    public class EmployeeServices
    {
        readonly HttpClient _client;
        readonly string baseUrl = "https://localhost:7181/";
        readonly JwtHeaderHelper _jwtHelper;
        public EmployeeServices(IHttpClientFactory client,JwtHeaderHelper jwtHeaderHelper) 
        {
            _client =client.CreateClient();
            _client.BaseAddress = new Uri(baseUrl);
            _jwtHelper = jwtHeaderHelper;
            _jwtHelper.AddJwtToHeaders(_client.DefaultRequestHeaders);
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            var employees = await _client.GetFromJsonAsync<List<Employee>>($"GetAllEmployees");
            if(employees == null )
            {
                throw new Exception("There are no Employee records");
            }
            return employees;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var employee= await _client.GetFromJsonAsync<Employee>($"GetEmployeesById/" + employeeId);
            if (employee == null)
            {
                throw new Exception($"There are no Employee record found with id{employeeId}");
            }
            return employee;

        }

        public async Task<bool> AddEmployeeAsync(Employee employee) 
        {
            var result = await _client.PostAsJsonAsync($"CreateEmployee", employee);
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> EditEmployeeAsync(Employee employee)
        {
            var respose = await _client.PutAsJsonAsync<Employee>($"UpdateEmployee", employee);
            return respose.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> DeletEmployeeAsync(int id)
        {
            var respose = await _client.DeleteAsync($"DeleteEmployee?id=" + id);
            return respose.StatusCode == System.Net.HttpStatusCode.OK;
        }
        public async Task<bool> PatchEmployee(JsonPatchDocument patchDocument,int id)
        {
            var serializeDoc = JsonConvert.SerializeObject(patchDocument);
            var request = new StringContent(serializeDoc, Encoding.UTF8, "application/json-patch+json");
            var PatchUpdate = await _client.PatchAsync("PatchEmployee/"+id, request);
            return PatchUpdate.IsSuccessStatusCode;
        }
    }
}
