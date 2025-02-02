using System.Security.Claims;

namespace WebApiAbstraction
{
    public interface ITokenService
    {
        Task<string> GenerateToken(IEnumerable<Claim> claims);
        Task<string> RefreshToken(string userId);
        Task<bool> ValidateRefreshToken(string userId,string refreshToken);
        Task RevokeRefreshToken(string userId);
        Task StoreAccessToken(string userId,string accessToken);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
