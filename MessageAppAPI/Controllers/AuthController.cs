using System.Security.Claims;
using MessageAppAPI.Abstractions.Services;
using MessageAppAPI.Dtos.Login;
using MessageAppAPI.Dtos.User;
using MessageAppAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var token = await _authService.LoginAsync(loginRequest.UsernameOrEmail, loginRequest.Password,
                365 * 24 * 60 * 60);
            return Ok(new { AccessToken = token.AccessToken });
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterUser dto)
        {
            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Email,
                Email = dto.Email,
                Role = "User"
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                return Ok(true);
            }

            return BadRequest(result.Errors);
        }

    }
}