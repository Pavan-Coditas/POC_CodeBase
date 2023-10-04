using EmployeeApiConsumer.Helpers;
using EmployeeApiConsumer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace EmployeeApiConsumer.Services.FilterData
{
    public class FilterDataService
    {
        private readonly HttpClient _client;
        private readonly string baseUrl;
        private readonly JwtHeaderHelper _jwtHelper;
        private readonly IConfiguration _configuration;

        public FilterDataService(HttpClient client, JwtHeaderHelper jwtHeaderHelper, IConfiguration configuration)
        {
            _configuration = configuration;
            baseUrl = _configuration.GetValue<string>("BaseUrl");
            _client = client;
            _client.BaseAddress = new Uri(baseUrl);
            _jwtHelper = jwtHeaderHelper;
            _jwtHelper.AddJwtToHeaders(_client.DefaultRequestHeaders);
        }

        public async Task<object> GetFilterData(SPPararm parameters)
        {
            var result =await _client.PostAsJsonAsync($"GetFilterdData?spName=sp_GetFilteredData", parameters);
            var responseData = await result.Content.ReadAsStringAsync();
            var apiResult = JsonConvert.DeserializeObject<FilteredDataResult>(responseData);
            return apiResult!;
        }
    }
}
