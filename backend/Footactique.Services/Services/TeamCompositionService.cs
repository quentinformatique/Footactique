using Footactique.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Footactique.Services.Services
{
    /// <summary>
    /// Service implementation for managing football team compositions.
    /// </summary>
    public class TeamCompositionService(ApplicationDbContext context, ILogger<TeamCompositionService> logger) : ITeamCompositionService
    {
        /// <inheritdoc/>
        public async Task<List<TeamComposition>> GetTeamCompositionsAsync(string userId)
        {
            using (logger.BeginScope("GetTeamCompositions for UserId: {UserId}", userId))
            {
                logger.LogInformation("Fetching all team compositions for user {UserId}", userId);
                List<TeamComposition> result = await context.TeamCompositions
                    .Where(tc => tc.UserId == userId)
                    .Include(tc => tc.Players)
                    .ToListAsync();
                logger.LogInformation("Found {Count} compositions for user {UserId}", result.Count, userId);
                return result;
            }
        }

        /// <inheritdoc/>
        public async Task<TeamComposition> GetTeamCompositionAsync(string userId, int id)
        {
            using (logger.BeginScope("GetTeamComposition for UserId: {UserId}, CompositionId: {Id}", userId, id))
            {
                logger.LogInformation("Fetching team composition {Id} for user {UserId}", id, userId);
                TeamComposition composition = await context.TeamCompositions
                    .Include(tc => tc.Players)
                    .FirstOrDefaultAsync(tc => tc.Id == id && tc.UserId == userId);
                if (composition == null)
                {
                    logger.LogWarning("Team composition {Id} not found for user {UserId}", id, userId);
                }
                else
                {
                    logger.LogInformation("Team composition {Id} found for user {UserId}", id, userId);
                }
                return composition;
            }
        }

        /// <inheritdoc/>
        public async Task<TeamComposition> CreateTeamCompositionAsync(string userId, TeamComposition composition)
        {
            using (logger.BeginScope("CreateTeamComposition for UserId: {UserId}", userId))
            {
                logger.LogInformation("Creating new team composition for user {UserId}", userId);
                composition.UserId = userId;
                context.TeamCompositions.Add(composition);
                await context.SaveChangesAsync();
                logger.LogInformation("Created team composition {Id} for user {UserId}", composition.Id, userId);
                return composition;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateTeamCompositionAsync(string userId, int id, TeamComposition updatedComposition)
        {
            using (logger.BeginScope("UpdateTeamComposition for UserId: {UserId}, CompositionId: {Id}", userId, id))
            {
                logger.LogInformation("Updating team composition {Id} for user {UserId}", id, userId);
                TeamComposition existing = await context.TeamCompositions
                    .Include(tc => tc.Players)
                    .FirstOrDefaultAsync(tc => tc.Id == id && tc.UserId == userId);
                if (existing == null)
                {
                    logger.LogWarning("Team composition {Id} not found for update (user {UserId})", id, userId);
                    return false;
                }
                existing.Name = updatedComposition.Name;
                existing.Formation = updatedComposition.Formation;
                existing.Description = updatedComposition.Description;
                existing.IsFavorite = updatedComposition.IsFavorite;
                context.PlayerPositions.RemoveRange(existing.Players);
                existing.Players = updatedComposition.Players;
                await context.SaveChangesAsync();
                logger.LogInformation("Updated team composition {Id} for user {UserId}", id, userId);
                return true;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteTeamCompositionAsync(string userId, int id)
        {
            using (logger.BeginScope("DeleteTeamComposition for UserId: {UserId}, CompositionId: {Id}", userId, id))
            {
                logger.LogInformation("Deleting team composition {Id} for user {UserId}", id, userId);
                TeamComposition existing = await context.TeamCompositions
                    .FirstOrDefaultAsync(tc => tc.Id == id && tc.UserId == userId);
                if (existing == null)
                {
                    logger.LogWarning("Team composition {Id} not found for deletion (user {UserId})", id, userId);
                    return false;
                }
                context.TeamCompositions.Remove(existing);
                await context.SaveChangesAsync();
                logger.LogInformation("Deleted team composition {Id} for user {UserId}", id, userId);
                return true;
            }
        }
    }
} 