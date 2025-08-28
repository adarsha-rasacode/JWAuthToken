namespace JwtAuthDotNet9.Services
{
    public class RefreshTokenRequestDto
    {
        public object RefreshToken { get; internal set; }
        public object UserId { get; internal set; }
    }
}