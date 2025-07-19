namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// Data Transfer Object for reading a player position.
    /// </summary>
    public class PlayerPositionDto
    {
        /// <summary>
        /// Player position ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the player.
        /// </summary>
        public required string PlayerName { get; set; }

        /// <summary>
        /// Position label (e.g., "Left Winger").
        /// </summary>
        public required string Position { get; set; }

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