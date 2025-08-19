namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a team composition.
    /// </summary>
    public class CreateTeamCompositionDto
    {
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
        public required List<CreatePlayerPositionDto> Players { get; set; }
    }
} 