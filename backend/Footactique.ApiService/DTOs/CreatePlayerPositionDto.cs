namespace Footactique.Api.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating or updating a player position.
    /// </summary>
    public class CreatePlayerPositionDto
    {
        /// <summary>
        /// Name of the player.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Position label (e.g., "Left Winger").
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Player's jersey number (optional).
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// X coordinate on the field (normalized, 0.0 = left, 1.0 = right).
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y coordinate on the field (normalized, 0.0 = bottom, 1.0 = top).
        /// </summary>
        public float Y { get; set; }
    }
}