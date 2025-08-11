namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// DTO for user profile information.
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// User's unique identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// User's username.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Date when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
} 