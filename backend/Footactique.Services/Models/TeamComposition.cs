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
        /// OIDC User ID (subject) who owns this composition.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// List of player positions in this composition.
        /// </summary>
        public List<PlayerPosition> Players { get; set; }
    }
} 