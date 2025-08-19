namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// DTO pour la lecture d'une position de joueur.
    /// </summary>
    public class PlayerPositionDto
    {
        /// <summary>
        /// Identifiant de la position du joueur.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du joueur.
        /// </summary>
        public required string PlayerName { get; set; }

        /// <summary>
        /// Libellé de la position (ex. « Ailier gauche »).
        /// </summary>
        public required string Position { get; set; }

        /// <summary>
        /// Numéro de maillot (optionnel).
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// Couleur du joueur sur le terrain (code hexadécimal).
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Coordonnée X (normalisée, 0.0 = gauche, 1.0 = droite).
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Coordonnée Y (normalisée, 0.0 = bas, 1.0 = haut).
        /// </summary>
        public float Y { get; set; }
    }
}