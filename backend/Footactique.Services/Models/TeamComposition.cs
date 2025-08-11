using System.Collections.Generic;

namespace Footactique.Services.Models
{
    /// <summary>
    /// Represents a football team composition (lineup) for a user.
    /// </summary>
    public class TeamComposition
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the team composition (e.g., "My 4-3-3 Lineup").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Formation string (e.g., "4-3-3").
        /// </summary>
        public string Formation { get; set; }

        /// <summary>
        /// Description of the team composition (optional).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Whether this composition is marked as favorite.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// The ID of the user who owns this team composition.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// List of player positions in this composition.
        /// </summary>
        public List<PlayerPosition> Players { get; set; }

        /// <summary>
        /// Creation date of the composition.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last update date of the composition.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
} 