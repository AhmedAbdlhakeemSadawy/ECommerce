using Microsoft.AspNetCore.Identity;
using WebApiAbstraction.Role_Authuntication;

namespace ECommwerceWebAPI.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<IdentityUser> userManager;

        public UserRoleService(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<List<string>> GetRolesForUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new List<string>(); // Return empty list if user is not found
            }

            return (await userManager.GetRolesAsync(user)).ToList();
        }
    }
}
