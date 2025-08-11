using Microsoft.AspNetCore.Identity;

namespace Footactique.Services.Models
{
    /// <summary>
    /// Application user entity for authentication and authorization.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Utilise les propriétés par défaut d'IdentityUser
        // UserName, Email, etc. sont déjà disponibles
    }
} 