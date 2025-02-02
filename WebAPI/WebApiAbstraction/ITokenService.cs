using System.Security.Claims;

namespace WebApiAbstraction
{
    public interface ITokenService
    {
        Task<string> GenerateToken(IEnumerable<Claim> claims);
        Task<string> RefreshToken(string userId);
        Task<bool> ValidateRefreshToken(string userId,string refreshToken);
        Task<bool> ValidateAccessToken(string userId,string refreshToken);
        Task RevokeRefreshToken(string userId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
