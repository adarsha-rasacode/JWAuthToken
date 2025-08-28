using JWAuthTokenDotNet9.Entities;
using JWAuthTokenDotNet9.Models;
using JWAuthTokenDotNet9.Services;
using Microsoft.AspNetCore.Mvc;

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

            if (user is null ) 
                {
                return BadRequest("Username already exists");
                 }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request) // Added 'request' parameter to the method
        {
            var token =await authService.LoginAsync(request);
            if(token is null )
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(token);

        }

        
    }
}