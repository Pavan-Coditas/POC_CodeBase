using EmployeeManagement.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EmployeeManagement.Api.CustomeMiddlewares
{
    public class JwtUserClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtUserClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null)
            {
                var token = authHeader.Split(" ")[1];
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                UserContext.UserName = usernameClaim!.Value;
            }
            var ipAddress = context.Request.Headers["IpAddress"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                UserContext.IpAddress = ipAddress;
            }
            await _next(context);
        }

    }
}
