using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAbstraction;

namespace ECommwerceWebAPI.Services
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration configuration;
        private readonly IDistributedCache cache;
        public TokenService(IConfiguration configuration, IDistributedCache cache)
        {
            this.configuration = configuration;
            this.cache = cache;
        }


        public async Task<string> RefreshToken(string userId)
        {
            var refreshToken = Guid.NewGuid().ToString();

            // Store refresh token in Redis with an expiration time (7 days)
            await cache.SetStringAsync(
                $"RefreshToken:{userId}",
                refreshToken,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                }
            );

            return refreshToken;
        }

        public async Task RevokeRefreshToken(string userId)
        {
            await cache.RemoveAsync($"RefreshToken:{userId}");
        }

        public async Task<bool> ValidateRefreshToken(string userId, string refreshToken)
        {
            var cachedToken = await cache.GetStringAsync($"RefreshToken:{userId}");
            return cachedToken == refreshToken;
        }

        public Task<string> GenerateToken(IEnumerable<Claim> claims)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateAccessToken(string userId, string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task RevokeAccessToken(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
