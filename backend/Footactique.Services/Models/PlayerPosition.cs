namespace Footactique.Services.Models
{
    /// <summary>
    /// Représente un joueur placé à une position spécifique sur le terrain dans une composition d'équipe.
    /// </summary>
    public class PlayerPosition
    {
        /// <summary>
        /// Clé primaire.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Clé étrangère vers la composition d'équipe parente.
        /// </summary>
        public int TeamCompositionId { get; set; }

        /// <summary>
        /// Nom du joueur.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Libellé de la position (ex. « Ailier gauche »).
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Numéro de maillot du joueur (optionnel).
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// Couleur du joueur sur le terrain (code hexadécimal).
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Coordonnée X sur le terrain (normalisée, 0.0 = gauche, 1.0 = droite).
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Coordonnée Y sur le terrain (normalisée, 0.0 = bas, 1.0 = haut).
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Propriété de navigation vers la composition d'équipe parente.
        /// </summary>
        public TeamComposition TeamComposition { get; set; }
    }
}