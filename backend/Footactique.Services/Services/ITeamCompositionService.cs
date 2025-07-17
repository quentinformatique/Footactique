using Footactique.Services.Models;

namespace Footactique.Services.Services
{
    /// <summary>
    /// Service interface for managing football team compositions.
    /// </summary>
    public interface ITeamCompositionService
    {
        /// <summary>
        /// Gets all team compositions for a specific user.
        /// </summary>
        /// <param name="userId">The OIDC user ID.</param>
        /// <returns>List of team compositions.</returns>
        Task<List<TeamComposition>> GetTeamCompositionsAsync(string userId);

        /// <summary>
        /// Gets a specific team composition by ID for a user.
        /// </summary>
        /// <param name="userId">The OIDC user ID.</param>
        /// <param name="id">The team composition ID.</param>
        /// <returns>The team composition, or null if not found.</returns>
        Task<TeamComposition> GetTeamCompositionAsync(string userId, int id);

        /// <summary>
        /// Creates a new team composition for a user.
        /// </summary>
        /// <param name="userId">The OIDC user ID.</param>
        /// <param name="composition">The team composition to create.</param>
        /// <returns>The created team composition.</returns>
        Task<TeamComposition> CreateTeamCompositionAsync(string userId, TeamComposition composition);

        /// <summary>
        /// Updates an existing team composition for a user.
        /// </summary>
        /// <param name="userId">The OIDC user ID.</param>
        /// <param name="id">The team composition ID.</param>
        /// <param name="composition">The updated team composition.</param>
        /// <returns>True if updated, false if not found.</returns>
        Task<bool> UpdateTeamCompositionAsync(string userId, int id, TeamComposition composition);

        /// <summary>
        /// Deletes a team composition for a user.
        /// </summary>
        /// <param name="userId">The OIDC user ID.</param>
        /// <param name="id">The team composition ID.</param>
        /// <returns>True if deleted, false if not found.</returns>
        Task<bool> DeleteTeamCompositionAsync(string userId, int id);
    }
} 