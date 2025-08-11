namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// DTO for user registration.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// User username.
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// User email address.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// User password.
        /// </summary>
        public required string Password { get; set; }
    }
} 