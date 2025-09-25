using ECommwerceWebAPI.Services;
using System.IO;
using System.Security.Claims;
using WebApiAbstraction;

namespace ECommwerceWebAPI.Middlewares
{
    public class AccessTokenValidationMiddleware : IMiddleware
    {
        private readonly ITokenService tokenService;
        private readonly ILogger<AccessTokenValidationMiddleware> logger;

        public AccessTokenValidationMiddleware( ITokenService tokenService, ILogger<AccessTokenValidationMiddleware> logger)
        {
            this.tokenService = tokenService;
            this.logger = logger;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            if (!context.Request.Path.StartsWithSegments("/api/Account/login", StringComparison.OrdinalIgnoreCase) 
                && !context.Request.Path.StartsWithSegments("/api/Account/refresh_token", StringComparison.OrdinalIgnoreCase)
                 && !context.Request.Path.StartsWithSegments("/api/Account/register", StringComparison.OrdinalIgnoreCase))
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                logger.LogInformation("UserID  is: " + userId.ToString());

                logger.LogInformation("Cached Token is: " + token.ToString());


                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) ||
                    !await tokenService.ValidateAccessToken(userId, token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }


            await next(context);
        }
    }
}
