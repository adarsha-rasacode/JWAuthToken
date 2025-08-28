using JWAuthTokenDotNet9.Data;
using JWAuthTokenDotNet9.Entities;
using JWAuthTokenDotNet9.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWAuthTokenDotNet9.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(UserDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // LOGIN ---------------------------------------------------------------
        async Task<string?> IAuthService.LoginAsync(UserDto request)
        {
            // 1) Find user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null)
                return null;

            // 2) Verify password
            var verify = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verify == PasswordVerificationResult.Failed)
                return null; // controller can return 401


            // 3) Issue JWT
            return CreateToken(user);
        }

        // REGISTER ------------------------------------------------------------
        // NOTE: returning User? so we can return null when username exists
        public async Task<User?> RegisterAsync(UserDto request)
        {
            // 1) Ensure unique username
            var exists = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (exists)
                return null; // caller can translate to 409/BadRequest, etc.

            // 2) Create entity and hash password
            var user = new User
            {
                Username = request.Username
            };

            // Pass 'user' instance into hasher (best practice)
            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            // 3) Save
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // JWT CREATION --------------------------------------------------------
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                // add roles/other claims here if you have them
            };

            // FIX: use _configuration (remove the stray 'configuration' field)
            var keyBytes = Encoding.UTF8.GetBytes(
                _configuration.GetValue<string>("AppSettings:Token")!
            );
            var key = new SymmetricSecurityKey(keyBytes);

            // HS512 matches your earlier alg example; HS256 is also fine
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),  // 1-day token
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
