using JWAuthTokenDotNet9.Entities;
using JWAuthTokenDotNet9.Models;
using JWAuthTokenDotNet9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace JWAuthTokenDotNet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {


        //user Registration Endpoint
        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);

            if (user is null)
            {
                return BadRequest("Username already exists");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request) // Added 'request' parameter to the method
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(result);

        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request) // Added 'request' parameter to the method
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token");
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("If you see this, you are authenticated!");
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("you are authenticated as the Admin ");
        }
    }
}