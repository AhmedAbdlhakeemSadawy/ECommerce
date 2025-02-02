using ECommwerceWebAPI.Services;
using System.Security.Claims;
using WebApiAbstraction;

namespace ECommwerceWebAPI.Middlewares
{
    public class AccessTokenValidationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ITokenService tokenService;

        public AccessTokenValidationMiddleware(RequestDelegate next, ITokenService tokenService)
        {
            this.next = next;
            this.tokenService = tokenService;
        }

        public async Task Invoke(HttpContext context)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) ||
                !await tokenService.ValidateAccessToken(userId, token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            await next(context);
        }
    }
}
