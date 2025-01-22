using Microsoft.AspNetCore.Authorization;

namespace ECommwerceWebAPI.Role_Requirements_Authorization
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public string RequiredRole { get; }

        public RoleRequirement(string requiredRole)
        {
            RequiredRole = requiredRole;
        }
    }
}
