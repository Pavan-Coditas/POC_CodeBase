using Quartz;

namespace EmployeeApiConsumer.Jobs
{
    public class HealthCheckJob : IJob
    {
        private readonly HttpClient _httpClient=new HttpClient();
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"This job executed at {DateTime.Now}");
            var response = await _httpClient.GetAsync($"https://localhost:7181/api/HealthCheck/HealthCheck");
            Console.WriteLine(response.StatusCode);
        }
    }
}
