using JWAuthTokenDotNet9.Entities;
using JWAuthTokenDotNet9.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JWAuthTokenDotNet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new();


        //User Registration Endpoint
        [HttpPost("Register")]
        public ActionResult<User> Register(UserDto request)
        {
            var hashedPassword = new PasswordHasher<User>() // this make the  password hashed and secure 
                .HashPassword(user, request.Password); //

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDto request) // Added 'request' parameter to the method
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User not Found ");
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(User, User.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest("Wrong password.");
            }


            string token = "sucess";

            return Ok(token);

        }
    }