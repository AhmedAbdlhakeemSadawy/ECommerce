using Azure.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAbstraction;

namespace ECommwerceWebAPI.Services
{
    public class TokenServiceRedis : ITokenService
    {

        private readonly IConfiguration configuration;
        private readonly IDistributedCache cache;

        public TokenServiceRedis(IConfiguration configuration, IDistributedCache cache)
        {
            this.configuration = configuration;
            this.cache = cache;
        }


        public async Task<string> RefreshToken(string userId)
        {
            var refreshToken = Guid.NewGuid().ToString();

            // Store refresh token in Redis with an expiration time (7 days)
            await cache.SetStringAsync(
                userId + "_RefreshToken",
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
            await cache.RemoveAsync(userId + "_RefreshToken");
        }

        public async Task<bool> ValidateRefreshToken(string userId, string refreshToken)
        {
            var cachedToken = await cache.GetStringAsync(userId + "_RefreshToken");
            return cachedToken == refreshToken;
        }

        public async Task<string> GenerateToken(IEnumerable<Claim> claims)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(jwtSettings["TimeZone"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var durationInMinutes = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo).AddMinutes(int.Parse(jwtSettings["DurationInMinutes"])).Minute;



            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);
            var userId = claims.FirstOrDefault(clm => clm.Type == ClaimTypes.NameIdentifier).Value;
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var expiryTime = TimeSpan.FromDays(1);

            await cache.SetStringAsync(userId + "_AccessToken", accessToken, new DistributedCacheEntryOptions
           {
               AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
           });


            return accessToken;
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

        public async Task<bool> ValidateAccessToken(string userId, string accessToken)
        {
           
            var cachedToken = await cache.GetStringAsync(userId + "_AccessToken");

            return cachedToken == accessToken;
        }

        public async Task RevokeAccessToken(string userId)
        {
            await cache.RemoveAsync(userId + "_AccessToken");

        }
    }
}
