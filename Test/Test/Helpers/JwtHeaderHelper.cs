using System.Net.Http.Headers;

namespace EmployeeApiConsumer.Helpers
{
    public class JwtHeaderHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtHeaderHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddJwtToHeaders(HttpRequestHeaders headers)
        {
            var token = _httpContextAccessor!.HttpContext!.Session.GetString("token")!;
            var ipAddress = _httpContextAccessor!.HttpContext!.Items["IpAddress"]!.ToString();

            if (!string.IsNullOrEmpty(token))
            {
                headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            if (!string.IsNullOrEmpty(ipAddress))
            {
                headers.Add("IpAddress", ipAddress);
            }
        }


    }
}
