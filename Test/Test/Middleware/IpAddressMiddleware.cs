using System.Net;
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
            string ipAddress = context.Request.Headers["X-Forwarded-For"]!;
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // Filter for Ethernet or Wi-Fi interfaces
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
                    {
                        // Check for IPv4 address and exclude loopback and link-local addresses
                        if (ipAddressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(ipAddressInfo.Address) &&
                            !ipAddressInfo.Address.IsIPv6LinkLocal)
                        {
                            ipAddress = ipAddressInfo.Address.ToString();
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    break;
                }
            }
            context.Items["IpAddress"] = ipAddress;
            await _requestDelegate(context);
        }
    }
}
