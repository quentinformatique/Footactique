namespace Footactique.Contracts.DTOs
{
    /// <summary>
    /// DTO pour la lecture d'une composition d'équipe.
    /// </summary>
    public class TeamCompositionDto
    {
        /// <summary>
        /// Identifiant de la composition.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom de la composition.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Formation (ex. « 4-3-3 »).
        /// </summary>
        public required string Formation { get; set; }

        /// <summary>
        /// Description de la composition (optionnel).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Indique si la composition est favorite.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Liste des positions des joueurs.
        /// </summary>
        public required List<PlayerPositionDto> Players { get; set; }

        /// <summary>
        /// Date de création.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de dernière mise à jour.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}