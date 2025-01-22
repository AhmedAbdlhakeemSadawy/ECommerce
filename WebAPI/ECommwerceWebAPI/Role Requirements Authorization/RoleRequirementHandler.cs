using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApiAbstraction.Role_Authuntication;

namespace ECommwerceWebAPI.Role_Requirements_Authorization
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly IUserRoleService userRoleService;

        public RoleRequirementHandler(IUserRoleService userRoleService)
        {
            this.userRoleService = userRoleService;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {

            if (!context.User.Identity?.IsAuthenticated == true)
            {
                return;
            }

            var userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (userRoles.Contains(requirement.RequiredRole))
            {
                context.Succeed(requirement);  // Role found in the token
                return;
            }

            var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            if (userId != null)
            {
                var roles = await userRoleService.GetRolesForUserAsync(userId);
                if (roles.Contains(requirement.RequiredRole))
                {
                    context.Succeed(requirement); // Role found in database
                }
            }

        }
    }
}
