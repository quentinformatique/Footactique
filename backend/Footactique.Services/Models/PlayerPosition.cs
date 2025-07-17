namespace Footactique.Services.Models
{
    /// <summary>
    /// Represents a player placed at a specific position on the field in a team composition.
    /// </summary>
    public class PlayerPosition
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the parent TeamComposition.
        /// </summary>
        public int TeamCompositionId { get; set; }

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

        /// <summary>
        /// Navigation property to parent TeamComposition.
        /// </summary>
        public TeamComposition TeamComposition { get; set; }
    }
} 