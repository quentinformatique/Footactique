using System.Security.Claims;
using Footactique.Api.DTOs;
using Footactique.Services.Models;
using Footactique.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Footactique.Api.Controllers
{
    /// <summary>
    /// API controller for managing football team compositions.
    /// </summary>
    /// <param name="service">The team composition business service.</param>
    /// <param name="logger">The logger instance.</param>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamCompositionsController(ITeamCompositionService service, ILogger<TeamCompositionsController> logger) : ControllerBase
    {
        /// <summary>
        /// Gets all team compositions for the authenticated user.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamCompositionDto>>> GetTeamCompositions()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            using (logger.BeginScope("GetTeamCompositions for UserId: {UserId}", userId))
            {
                try
                {
                    logger.LogInformation("Request to get all team compositions for user {UserId}", userId);
                    List<TeamComposition> compositions = await service.GetTeamCompositionsAsync(userId);
                    List<TeamCompositionDto> dtoList = compositions.Select(ToDto).ToList();
                    return Ok(dtoList);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while getting team compositions for user {UserId}", userId);
                    return Problem("An error occurred while retrieving team compositions.", statusCode: 500);
                }
            }
        }

        /// <summary>
        /// Gets a specific team composition by ID for the authenticated user.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamCompositionDto>> GetTeamComposition(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            using (logger.BeginScope("GetTeamComposition for UserId: {UserId}, CompositionId: {Id}", userId, id))
            {
                try
                {
                    logger.LogInformation("Request to get team composition {Id} for user {UserId}", id, userId);
                    TeamComposition composition = await service.GetTeamCompositionAsync(userId, id);
                    if (composition == null)
                    {
                        logger.LogWarning("Team composition {Id} not found for user {UserId}", id, userId);
                        return NotFound();
                    }
                    TeamCompositionDto dto = ToDto(composition);
                    return Ok(dto);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while getting team composition {Id} for user {UserId}", id, userId);
                    return Problem("An error occurred while retrieving the team composition.", statusCode: 500);
                }
            }
        }

        /// <summary>
        /// Creates a new team composition for the authenticated user.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TeamCompositionDto>> CreateTeamComposition(CreateTeamCompositionDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            using (logger.BeginScope("CreateTeamComposition for UserId: {UserId}", userId))
            {
                try
                {
                    logger.LogInformation("Request to create a new team composition for user {UserId}", userId);
                    List<PlayerPosition> players = dto.Players.Select(p => new PlayerPosition
                    {
                        PlayerName = p.PlayerName,
                        Position = p.Position,
                        Number = p.Number,
                        X = p.X,
                        Y = p.Y
                    }).ToList();
                    TeamComposition composition = new TeamComposition
                    {
                        Name = dto.Name,
                        Formation = dto.Formation,
                        Players = players
                    };
                    TeamComposition created = await service.CreateTeamCompositionAsync(userId, composition);
                    TeamCompositionDto resultDto = ToDto(created);
                    logger.LogInformation("Successfully created team composition {Id} for user {UserId}", created.Id, userId);
                    return CreatedAtAction(nameof(GetTeamComposition), new { id = created.Id }, resultDto);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while creating team composition for user {UserId}", userId);
                    return Problem("An error occurred while creating the team composition.", statusCode: 500);
                }
            }
        }

        /// <summary>
        /// Updates an existing team composition for the authenticated user.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeamComposition(int id, UpdateTeamCompositionDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            using (logger.BeginScope("UpdateTeamComposition for UserId: {UserId}, CompositionId: {Id}", userId, id))
            {
                try
                {
                    logger.LogInformation("Request to update team composition {Id} for user {UserId}", id, userId);
                    List<PlayerPosition> newPlayers = dto.Players.Select(p => new PlayerPosition
                    {
                        PlayerName = p.PlayerName,
                        Position = p.Position,
                        Number = p.Number,
                        X = p.X,
                        Y = p.Y
                    }).ToList();
                    TeamComposition updated = new TeamComposition
                    {
                        Name = dto.Name,
                        Formation = dto.Formation,
                        Players = newPlayers
                    };
                    bool updatedResult = await service.UpdateTeamCompositionAsync(userId, id, updated);
                    if (!updatedResult)
                    {
                        logger.LogWarning("Team composition {Id} not found for update (user {UserId})", id, userId);
                        return NotFound();
                    }
                    logger.LogInformation("Successfully updated team composition {Id} for user {UserId}", id, userId);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while updating team composition {Id} for user {UserId}", id, userId);
                    return Problem("An error occurred while updating the team composition.", statusCode: 500);
                }
            }
        }

        /// <summary>
        /// Deletes a team composition for the authenticated user.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamComposition(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            using (logger.BeginScope("DeleteTeamComposition for UserId: {UserId}, CompositionId: {Id}", userId, id))
            {
                try
                {
                    logger.LogInformation("Request to delete team composition {Id} for user {UserId}", id, userId);
                    bool deleted = await service.DeleteTeamCompositionAsync(userId, id);
                    if (!deleted)
                    {
                        logger.LogWarning("Team composition {Id} not found for deletion (user {UserId})", id, userId);
                        return NotFound();
                    }
                    logger.LogInformation("Successfully deleted team composition {Id} for user {UserId}", id, userId);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while deleting team composition {Id} for user {UserId}", id, userId);
                    return Problem("An error occurred while deleting the team composition.", statusCode: 500);
                }
            }
        }

        // --- Mapping helpers ---
        private static TeamCompositionDto ToDto(TeamComposition tc)
        {
            List<PlayerPositionDto> playerDtos = tc.Players?.Select(p => new PlayerPositionDto
            {
                Id = p.Id,
                PlayerName = p.PlayerName,
                Position = p.Position,
                Number = p.Number,
                X = p.X,
                Y = p.Y
            }).ToList() ?? new List<PlayerPositionDto>();
            TeamCompositionDto dto = new TeamCompositionDto
            {
                Id = tc.Id,
                Name = tc.Name,
                Formation = tc.Formation,
                Players = playerDtos
            };
            return dto;
        }
    }
}