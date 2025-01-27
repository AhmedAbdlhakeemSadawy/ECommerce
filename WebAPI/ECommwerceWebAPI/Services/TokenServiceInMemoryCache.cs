using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAbstraction;

namespace ECommwerceWebAPI.Services
{
    public class TokenServiceInMemoryCache : ITokenService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IConfiguration configuration;

        public TokenServiceInMemoryCache(IMemoryCache memoryCache, IConfiguration configuration)
        {
            this.memoryCache = memoryCache;
            this.configuration = configuration;
        }
        public async Task<string> GenerateToken(IEnumerable<Claim> claims)
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> RefreshToken(string userId)
        {
            var refreshToken = Guid.NewGuid().ToString();
            await  Task.FromResult(memoryCache.Set(userId, refreshToken, TimeSpan.FromDays(7)));
            return refreshToken;
        }

        public Task RevokeRefreshToken(string userId)
        {
            memoryCache.Remove(userId);
            return Task.CompletedTask;
        }

        public Task<bool> ValidateRefreshToken(string userId, string refreshToken)
        {
            var cachedToken = memoryCache.Get(userId);
            return Task.FromResult(cachedToken == refreshToken);
        }
    }
}
