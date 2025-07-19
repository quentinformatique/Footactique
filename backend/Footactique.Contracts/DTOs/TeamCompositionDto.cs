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
        /// List of player positions in this composition.
        /// </summary>
        public required List<PlayerPositionDto> Players { get; set; }
    }
} 