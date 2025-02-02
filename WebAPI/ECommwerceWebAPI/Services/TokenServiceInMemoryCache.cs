using Azure.Core;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
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

            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(jwtSettings["TimeZone"]);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo).AddMinutes(int.Parse(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds);
            var userId = claims.FirstOrDefault(clm => clm.Type == ClaimTypes.NameIdentifier).Value;
            memoryCache.Set(userId + "_AccessToken", token, TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo).AddMinutes(int.Parse(jwtSettings["DurationInMinutes"])));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = false // Allow expired tokens
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<string> RefreshToken(string userId)
        {
            var refreshToken = Guid.NewGuid().ToString();
            await  Task.FromResult(memoryCache.Set(userId + "_RefreshToken", refreshToken, TimeSpan.FromDays(7)));
            return refreshToken;
        }

        public Task RevokeAccessToken(string userId)
        {
            memoryCache.Remove(userId + "_AccessToken");
            return Task.CompletedTask;
        }

        public Task RevokeRefreshToken(string userId)
        {
            memoryCache.Remove(userId + "_RefreshToken");
            return Task.CompletedTask;
        }

        public Task<bool> ValidateAccessToken(string userId, string accessToken)
        {
            var cachedToken = memoryCache.Get(userId + "_AccessToken");
            return Task.FromResult(cachedToken.ToString() == accessToken);
        }

        public Task<bool> ValidateRefreshToken(string userId, string refreshToken)
        {
            var cachedToken = memoryCache.Get(userId + "_RefreshToken");
            return Task.FromResult(cachedToken.ToString() == refreshToken);
        }
    }
}
