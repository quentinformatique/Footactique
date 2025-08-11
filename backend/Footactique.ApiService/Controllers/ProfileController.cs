using System.Security.Claims;
using Footactique.Contracts.DTOs;
using Footactique.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Footactique.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IAuthService authService, ILogger<ProfileController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in claims");
                    return Unauthorized();
                }

                _logger.LogInformation("Getting profile for user: {UserId}", userId);
                var profile = await _authService.GetUserProfileAsync(userId);
                
                if (profile == null)
                {
                    _logger.LogWarning("Profile not found for user: {UserId}", userId);
                    return NotFound();
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in claims");
                    return Unauthorized();
                }

                _logger.LogInformation("Updating profile for user: {UserId}", userId);
                var result = await _authService.UpdateUserProfileAsync(userId, dto);
                
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    _logger.LogWarning("Failed to update profile for user {UserId}. Errors: {Errors}", 
                        userId, string.Join(", ", errors));
                    return BadRequest(errors);
                }

                // Generate new JWT token with updated user information
                var newToken = await _authService.GenerateNewTokenAsync(userId);
                
                _logger.LogInformation("Profile updated successfully for user: {UserId}", userId);
                return Ok(new { token = newToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 