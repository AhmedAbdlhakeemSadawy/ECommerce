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

        #region login using cookie based authentication
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginModel model)
        //{
        //    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

        //    if (result.Succeeded)
        //        return Ok("Login successful.");

        //    return Unauthorized("Invalid login attempt.");
        //}
        #endregion


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
                new Claim(JwtRegisteredClaimNames.Sid, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
                // Add roles or custom claims as needed
            };


            var token = tokenService.GenerateToken(claims);

            return Ok(new { Token = token });

        }

        [HttpPost("logout")]
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // Signs the user out
            return Ok(new { message = "Successfully logged out." });
        }
    }
}
