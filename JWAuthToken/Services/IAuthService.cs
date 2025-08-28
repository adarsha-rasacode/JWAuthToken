using JWAuthTokenDotNet9.Entities;
using JWAuthTokenDotNet9.Models;

namespace JWAuthTokenDotNet9.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto user);
        Task<string?> LoginAsync(UserDto request); // method signature 
       
    }
}
