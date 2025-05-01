using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiAbstraction.Role_Authuntication
{
    public interface IUserRoleService
    {
        Task<List<string>> GetRolesForUserAsync(string userId);
    }
}
