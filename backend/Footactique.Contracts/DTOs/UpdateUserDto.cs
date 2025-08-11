namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// DTO for updating user information.
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// New username for the user.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// New email for the user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Current password for verification.
        /// </summary>
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// New password (optional).
        /// </summary>
        public string? NewPassword { get; set; }
    }
} 