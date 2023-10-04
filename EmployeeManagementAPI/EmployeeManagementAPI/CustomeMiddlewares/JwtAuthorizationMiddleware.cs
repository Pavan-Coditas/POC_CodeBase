using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace EmployeeManagement.Api.CustomeMiddlewares
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public JwtAuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var loginPath = "/Login";
            var regiterPath = "/Register";
            // Skip token validation for login requests
            if (context.Request.Path.Equals(loginPath, StringComparison.OrdinalIgnoreCase) || context.Request.Path.Equals(regiterPath, StringComparison.OrdinalIgnoreCase))
            {
                await next(context);
                return;
            }

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var token = authHeader.Split(" ")[1];
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
                };

                tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(ex.Message);
                return;
            }

            await next(context);
        }
    }
    public static class JwtAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtAuthorizationMiddleware>();
        }
    }
}
