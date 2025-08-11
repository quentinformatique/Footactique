namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// Data Transfer Object for reading a team composition.
    /// </summary>
    public class TeamCompositionDto
    {
        /// <summary>
        /// Team composition ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the team composition.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Formation string (e.g., "4-3-3").
        /// </summary>
        public required string Formation { get; set; }

        /// <summary>
        /// Description of the team composition (optional).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Whether this composition is marked as favorite.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// List of player positions in this composition.
        /// </summary>
        public required List<PlayerPositionDto> Players { get; set; }

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