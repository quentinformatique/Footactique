using Microsoft.AspNetCore.Identity;

namespace Footactique.Services.Models
{
    /// <summary>
    /// Application user entity for authentication and authorization.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Ajoute ici des propriétés personnalisées si besoin (ex: prénom, nom, etc.)
        // /// <summary>
        // /// User's first name.
        // /// </summary>
        // public string FirstName { get; set; }
    }
} 