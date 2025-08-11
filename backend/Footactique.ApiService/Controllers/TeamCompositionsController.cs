using System.Security.Claims;
using Footactique.Contracts.DTOs;
using Footactique.Services.Models;
using Footactique.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Footactique.Api.Controllers
{
    /// <summary>
    /// Controller for managing football team compositions.
    /// All endpoints require authentication via JWT Bearer token.
    /// </summary>
    /// <param name="teamCompositionService">The team composition service.</param>
    /// <param name="logger">The logger instance.</param>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class TeamCompositionsController(ITeamCompositionService teamCompositionService, ILogger<TeamCompositionsController> logger) : ControllerBase
    {
        /// <summary>
        /// Get all team compositions for the authenticated user.
        /// </summary>
        /// <returns>List of team compositions belonging to the current user.</returns>
        /// <response code="200">Returns the list of team compositions.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<TeamCompositionDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<List<TeamCompositionDto>>> GetTeamCompositions()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("GetTeamCompositions: User ID not found in claims");
                return Unauthorized();
            }

            logger.LogInformation("GetTeamCompositions: Request for user {UserId}", userId);
            List<TeamComposition> compositions = await teamCompositionService.GetTeamCompositionsAsync(userId);
            
            List<TeamCompositionDto> result = compositions.Select(c => new TeamCompositionDto
            {
                Id = c.Id,
                Name = c.Name,
                Formation = c.Formation,
                Players = c.Players.Select(p => new PlayerPositionDto
                {
                    Id = p.Id,
                    PlayerName = p.PlayerName,
                    Position = p.Position,
                    Number = p.Number,
                    Color = p.Color,
                    X = p.X,
                    Y = p.Y
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Get a specific team composition by ID for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the team composition.</param>
        /// <returns>The team composition if found and owned by the user.</returns>
        /// <response code="200">Returns the requested team composition.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the team composition is not found or doesn't belong to the user.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TeamCompositionDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TeamCompositionDto>> GetTeamComposition(int id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("GetTeamComposition: User ID not found in claims");
                return Unauthorized();
            }

            logger.LogInformation("GetTeamComposition: Request for composition {Id} by user {UserId}", id, userId);
            TeamComposition? composition = await teamCompositionService.GetTeamCompositionAsync(userId, id);
            
            if (composition == null)
            {
                logger.LogWarning("GetTeamComposition: Composition {Id} not found for user {UserId}", id, userId);
                return NotFound();
            }

            TeamCompositionDto result = new TeamCompositionDto
            {
                Id = composition.Id,
                Name = composition.Name,
                Formation = composition.Formation,
                Players = composition.Players.Select(p => new PlayerPositionDto
                {
                    Id = p.Id,
                    PlayerName = p.PlayerName,
                    Position = p.Position,
                    Number = p.Number,
                    Color = p.Color,
                    X = p.X,
                    Y = p.Y
                }).ToList()
            };

            return Ok(result);
        }

        /// <summary>
        /// Create a new team composition for the authenticated user.
        /// </summary>
        /// <param name="dto">The team composition data.</param>
        /// <returns>The created team composition.</returns>
        /// <response code="201">Returns the newly created team composition.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TeamCompositionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<TeamCompositionDto>> CreateTeamComposition([FromBody] CreateTeamCompositionDto dto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("CreateTeamComposition: User ID not found in claims");
                return Unauthorized();
            }

            logger.LogInformation("CreateTeamComposition: Request to create a new team composition for user {UserId}", userId);
            
            TeamComposition composition = new TeamComposition
            {
                Name = dto.Name,
                Formation = dto.Formation,
                Players = dto.Players.Select(p => new PlayerPosition
                {
                    PlayerName = p.PlayerName,
                    Position = p.Position,
                    Number = p.Number,
                    Color = p.Color,
                    X = p.X,
                    Y = p.Y
                }).ToList()
            };

            TeamComposition created = await teamCompositionService.CreateTeamCompositionAsync(userId, composition);
            logger.LogInformation("CreateTeamComposition: Successfully created team composition {Id} for user {UserId}", created.Id, userId);

            TeamCompositionDto result = new TeamCompositionDto
            {
                Id = created.Id,
                Name = created.Name,
                Formation = created.Formation,
                Players = created.Players.Select(p => new PlayerPositionDto
                {
                    Id = p.Id,
                    PlayerName = p.PlayerName,
                    Position = p.Position,
                    Number = p.Number,
                    Color = p.Color,
                    X = p.X,
                    Y = p.Y
                }).ToList()
            };

            return CreatedAtAction(nameof(GetTeamComposition), new { id = created.Id }, result);
        }

        /// <summary>
        /// Update an existing team composition for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the team composition to update.</param>
        /// <param name="dto">The updated team composition data.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the team composition was successfully updated.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the team composition is not found or doesn't belong to the user.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTeamComposition(int id, [FromBody] UpdateTeamCompositionDto dto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("UpdateTeamComposition: User ID not found in claims");
                return Unauthorized();
            }

            logger.LogInformation("UpdateTeamComposition: Request to update team composition {Id} for user {UserId}", id, userId);
            
            TeamComposition composition = new TeamComposition
            {
                Name = dto.Name,
                Formation = dto.Formation,
                Players = dto.Players.Select(p => new PlayerPosition
                {
                    PlayerName = p.PlayerName,
                    Position = p.Position,
                    Number = p.Number,
                    Color = p.Color,
                    X = p.X,
                    Y = p.Y
                }).ToList()
            };

            bool success = await teamCompositionService.UpdateTeamCompositionAsync(userId, id, composition);
            
            if (!success)
            {
                logger.LogWarning("UpdateTeamComposition: Composition {Id} not found for user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("UpdateTeamComposition: Successfully updated team composition {Id} for user {UserId}", id, userId);
            return NoContent();
        }

        /// <summary>
        /// Delete a team composition for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the team composition to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the team composition was successfully deleted.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the team composition is not found or doesn't belong to the user.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTeamComposition(int id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("DeleteTeamComposition: User ID not found in claims");
                return Unauthorized();
            }

            logger.LogInformation("DeleteTeamComposition: Request to delete team composition {Id} for user {UserId}", id, userId);
            
            bool success = await teamCompositionService.DeleteTeamCompositionAsync(userId, id);
            
            if (!success)
            {
                logger.LogWarning("DeleteTeamComposition: Composition {Id} not found for user {UserId}", id, userId);
                return NotFound();
            }

            logger.LogInformation("DeleteTeamComposition: Successfully deleted team composition {Id} for user {UserId}", id, userId);
            return NoContent();
        }
    }
}