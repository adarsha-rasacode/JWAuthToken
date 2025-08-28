namespace JwtAuthDotNet9.Services
{
    public class TokenResponseDto
    {
        public string AccessToken { get; internal set; }
        public string RefreshToken { get; internal set; }
    }
}