using Azure.Core;
using ECommerceWebApiDto;
using ECommwerceWebAPI.Models;
using ECommwerceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApiAbstraction;

namespace ECommwerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ITokenService tokenService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,ITokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                return Ok("User registered successfully.");
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("accessdenied")]
        public IActionResult AccessDenied()
        {
            return Unauthorized("You do not have access to this resource.");
        }

        //#region login using cookie based authentication
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginModel model)
        //{
        //    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

        //    if (result.Succeeded)
        //        return Ok("Login successful.");

        //    return Unauthorized("Invalid login attempt.");
        //}
        //#endregion


        #region Login using jwt token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Unauthorized(new { Message = "Invalid email or password." });

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
                // return Ok("Login successful.");
                return Unauthorized("Invalid login attempt.");


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
                // Add roles or custom claims as needed
            };


            var accessToken = tokenService.GenerateToken(claims);
            var refreshToken = tokenService.RefreshToken(user.Id);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });

        }

        #endregion

        #region LogOut using cookie based authentication

        //[HttpPost("logout")]
        //[Authorize] // Ensure the user is authenticated
        //public async Task<IActionResult> Logout()
        //{
        //    await signInManager.SignOutAsync(); // Signs the user out
        //    return Ok(new { message = "Successfully logged out." });
        //} 
        #endregion

        #region Logout for jwt authetication
        [HttpPost("logout")]
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await tokenService.RevokeAccessToken(userId);
            await tokenService.RevokeRefreshToken(userId);
            return Ok(new { message = "Successfully logged out." });
        } 
        #endregion


        [HttpPost("refresh_token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(tokenRequestDto.AccessToken);
            if (principal == null)
                return Unauthorized("Invalid token");

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Invalid token");

            bool validRefreshToken = await tokenService.ValidateRefreshToken(userId, tokenRequestDto.RefreshToken);
            if (!validRefreshToken)
                return Unauthorized("Invalid refresh token");


            var accessToken = tokenService.GenerateToken(principal.Claims);
            var refreshToken = tokenService.RefreshToken(userId);
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
