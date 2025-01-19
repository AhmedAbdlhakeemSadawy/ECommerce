using System.Security.Claims;

namespace WebApiAbstraction
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
    }
}
