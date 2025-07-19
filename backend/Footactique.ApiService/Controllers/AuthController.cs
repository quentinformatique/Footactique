using Footactique.Contracts.DTOs;
using Footactique.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Footactique.Api.Controllers
{
    /// <summary>
    /// Controller for authentication and user management operations.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    /// <param name="logger">The logger instance.</param>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
    {
        /// <summary>
        /// Register a new user. All new users are assigned the 'user' role by default.
        /// </summary>
        /// <param name="dto">Registration data for the new user.</param>
        /// <returns>Success message if registration is successful.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">If the registration data is invalid or user already exists.</response>
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Register: Invalid model state");
                return BadRequest(ModelState);
            }

            IdentityResult result = await authService.RegisterUserAsync(dto);

            if (!result.Succeeded)
            {
                logger.LogWarning("Register: Failed for {Email}. Errors: {Errors}", 
                    dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(result.Errors);
            }

            logger.LogInformation("Register: User {Email} created successfully", dto.Email);
            return Ok(new { message = "User registered successfully" });
        }

        /// <summary>
        /// Creates a new admin user. Only accessible to authenticated users with the 'admin' role.
        /// </summary>
        /// <param name="dto">Registration data for the new admin.</param>
        /// <returns>Success message if admin creation is successful.</returns>
        /// <response code="200">Admin user registered successfully.</response>
        /// <response code="400">If the registration data is invalid or user already exists.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have admin role.</response>
        [HttpPost("create-admin")]
        [Authorize(Roles = "admin")]
        [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger documentation
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("CreateAdmin: Invalid model state");
                return BadRequest(ModelState);
            }

            IdentityResult result = await authService.RegisterAdminAsync(dto);

            if (!result.Succeeded)
            {
                logger.LogWarning("CreateAdmin: Failed for {Email}. Errors: {Errors}", 
                    dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(result.Errors);
            }

            logger.LogInformation("CreateAdmin: Admin user {Email} created successfully", dto.Email);
            return Ok(new { message = "Admin user registered successfully" });
        }

        /// <summary>
        /// Login and get a JWT token and refresh token.
        /// </summary>
        /// <param name="dto">Login credentials.</param>
        /// <returns>JWT token and refresh token if login is successful.</returns>
        /// <response code="200">Returns the JWT token and refresh token.</response>
        /// <response code="400">If the login data is invalid.</response>
        /// <response code="401">If the credentials are invalid.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Login: Invalid model state");
                return BadRequest(ModelState);
            }

            (string jwt, string refreshToken, IdentityResult result) = await authService.LoginAsync(dto);

            if (!result.Succeeded)
            {
                logger.LogWarning("Login: Failed for {Email}. Errors: {Errors}", 
                    dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return Unauthorized(new { message = "Invalid credentials" });
            }

            logger.LogInformation("Login: User {Email} logged in successfully", dto.Email);
            return Ok(new AuthResponseDto { Token = jwt, RefreshToken = refreshToken });
        }

        /// <summary>
        /// Refresh JWT using a valid refresh token.
        /// </summary>
        /// <param name="dto">Refresh token data.</param>
        /// <returns>New JWT token and refresh token if refresh is successful.</returns>
        /// <response code="200">Returns the new JWT token and refresh token.</response>
        /// <response code="400">If the refresh token is invalid or expired.</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            (string jwt, string refreshToken, bool success, string? errorMessage) = await authService.RefreshTokenAsync(dto.RefreshToken);

            if (!success)
            {
                logger.LogWarning("Refresh: Failed with error: {ErrorMessage}", errorMessage);
                return BadRequest(new { message = errorMessage });
            }

            logger.LogInformation("Refresh: Token refreshed successfully");
            return Ok(new AuthResponseDto { Token = jwt, RefreshToken = refreshToken });
        }
    }
}