using System.Net.NetworkInformation;

namespace EmployeeApiConsumer.CustomeMiddlewares
{
    public class IpAddressMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        public IpAddressMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
          //string ipAddress = context.Request.Headers["X-Forwarded-For"];
          //  if (string.IsNullOrEmpty(ipAddress))
          //  {
          //      ipAddress = context.Connection.RemoteIpAddress!.MapToIPv4().ToString();
          //  }
          //  context.Items["IpAddress"] = ipAddress;

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                var properties = networkInterface.GetIPProperties();
                var ipv4Addresses = properties.UnicastAddresses
                    .Where(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(x => x.Address.ToString());
                if (ipv4Addresses.Any())
                {
                    var ip= ipv4Addresses.First();
                    context.Items["IpAddress"] = ip;
                }
            }
            await _requestDelegate(context);
        }
    }
}
