namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// Data Transfer Object for updating a team composition.
    /// </summary>
    public class UpdateTeamCompositionDto
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
        /// List of player positions in this composition.
        /// </summary>
        public required List<CreatePlayerPositionDto> Players { get; set; }
    }
} 