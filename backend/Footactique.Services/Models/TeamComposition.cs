using System.Collections.Generic;

namespace Footactique.Services.Models
{
    /// <summary>
    /// Représente une composition d'équipe (onze) pour un utilisateur.
    /// </summary>
    public class TeamComposition
    {
        /// <summary>
        /// Clé primaire.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom de la composition (ex. « Mon 4-3-3 »).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Formation (ex. « 4-3-3 »).
        /// </summary>
        public string Formation { get; set; }

        /// <summary>
        /// Description de la composition (optionnel).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Indique si la composition est marquée comme favorite.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur propriétaire de cette composition.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Liste des positions des joueurs dans cette composition.
        /// </summary>
        public List<PlayerPosition> Players { get; set; }

        /// <summary>
        /// Date de création de la composition.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de dernière mise à jour de la composition.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}