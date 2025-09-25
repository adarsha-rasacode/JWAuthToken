﻿
//plain text user and pass 
namespace JWAuthTokenDotNet9.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Username { get;  set; }
        public string? PasswordHash { get; set; }

        public string? Role { get; set; } 
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }


    }
}
