namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// DTO for requesting a new JWT using a refresh token.
    /// </summary>
    public class RefreshTokenRequestDto
    {
        /// <summary>
        /// The refresh token value.
        /// </summary>
        public required string RefreshToken { get; set; }
    }
} 