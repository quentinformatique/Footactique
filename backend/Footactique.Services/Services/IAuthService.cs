using System.Threading.Tasks;
using Footactique.Contracts.DTOs;
using Footactique.Services.Models;
using Microsoft.AspNetCore.Identity;

namespace Footactique.Services.Services
{
    /// <summary>
    /// Interface for authentication and user management services.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user with the 'user' role.
        /// </summary>
        Task<IdentityResult> RegisterUserAsync(RegisterDto dto);

        /// <summary>
        /// Registers a new admin user. Only callable by an admin.
        /// </summary>
        Task<IdentityResult> RegisterAdminAsync(RegisterDto dto);

        /// <summary>
        /// Authenticates a user and returns a JWT and refresh token.
        /// </summary>
        Task<(string Jwt, string RefreshToken, IdentityResult Result)> LoginAsync(LoginDto dto);

        /// <summary>
        /// Refreshes a JWT using a valid refresh token.
        /// </summary>
        Task<(string Jwt, string RefreshToken, bool Success, string? ErrorMessage)> RefreshTokenAsync(string refreshToken);
    }
} 