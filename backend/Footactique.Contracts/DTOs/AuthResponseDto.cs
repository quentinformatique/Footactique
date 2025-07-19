namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// DTO for authentication response (JWT and refresh token).
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// The JWT access token.
        /// </summary>
        public required string Token { get; set; }

        /// <summary>
        /// The refresh token.
        /// </summary>
        public required string RefreshToken { get; set; }
    }
} 