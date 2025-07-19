using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Footactique.Services.Models
{
    /// <summary>
    /// Refresh token entity for JWT authentication.
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Token value.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Expiration date of the refresh token.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Is the token revoked?
        /// </summary>
        public bool Revoked { get; set; }

        /// <summary>
        /// Foreign key to the user.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Navigation property to the user.
        /// </summary>
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
} 