namespace Footactique.Api.DTOs
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
        public string Name { get; set; }

        /// <summary>
        /// Formation string (e.g., "4-3-3").
        /// </summary>
        public string Formation { get; set; }

        /// <summary>
        /// List of player positions in this composition.
        /// </summary>
        public List<PlayerPositionDto> Players { get; set; }
    }
}