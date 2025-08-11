using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Footactique.Contracts.DTOs;
using Footactique.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Footactique.Services.Services
{
    /// <summary>
    /// Service for authentication and user management operations.
    /// </summary>
    /// <param name="userManager">The user manager for Identity operations.</param>
    /// <param name="signInManager">The sign-in manager for authentication.</param>
    /// <param name="roleManager">The role manager for role operations.</param>
    /// <param name="dbContext">The database context for refresh token storage.</param>
    /// <param name="configuration">The configuration for JWT settings.</param>
    /// <param name="logger">The logger instance.</param>
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext,
        IConfiguration configuration,
        ILogger<AuthService> logger) : IAuthService
    {
        /// <summary>
        /// Registers a new user with the 'user' role.
        /// </summary>
        public async Task<IdentityResult> RegisterUserAsync(RegisterDto dto)
        {
            logger.LogInformation("Registering new user with email: {Email}", dto.Email);
            
            ApplicationUser user = new ApplicationUser { UserName = dto.Username, Email = dto.Email };
            IdentityResult result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                logger.LogWarning("User registration failed for {Email}. Errors: {Errors}", 
                    dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return result;
            }

            // Always assign the 'user' role
            const string role = "user";
            if (!await roleManager.RoleExistsAsync(role))
            {
                IdentityResult roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Failed to create role {Role}. Errors: {Errors}", 
                        role, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    return roleResult;
                }
            }
            
            IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addRoleResult.Succeeded)
            {
                logger.LogError("Failed to add user {Email} to role {Role}. Errors: {Errors}", 
                    dto.Email, role, string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                return addRoleResult;
            }

            logger.LogInformation("User {Email} registered successfully with role {Role}", dto.Email, role);
            return IdentityResult.Success;
        }

        /// <summary>
        /// Registers a new admin user. Only callable by an admin.
        /// </summary>
        public async Task<IdentityResult> RegisterAdminAsync(RegisterDto dto)
        {
            logger.LogInformation("Registering new admin user with email: {Email}", dto.Email);
            
            ApplicationUser user = new ApplicationUser { UserName = dto.Username, Email = dto.Email };
            IdentityResult result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                logger.LogWarning("Admin registration failed for {Email}. Errors: {Errors}", 
                    dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return result;
            }

            // Always assign the 'admin' role
            const string role = "admin";
            if (!await roleManager.RoleExistsAsync(role))
            {
                IdentityResult roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Failed to create role {Role}. Errors: {Errors}", 
                        role, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    return roleResult;
                }
            }
            
            IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addRoleResult.Succeeded)
            {
                logger.LogError("Failed to add user {Email} to role {Role}. Errors: {Errors}", 
                    dto.Email, role, string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                return addRoleResult;
            }

            logger.LogInformation("Admin user {Email} registered successfully with role {Role}", dto.Email, role);
            return IdentityResult.Success;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT and refresh token.
        /// </summary>
        public async Task<(string Jwt, string RefreshToken, IdentityResult Result)> LoginAsync(LoginDto dto)
        {
            logger.LogInformation("Login attempt for user: {Email}", dto.Email);
            
            ApplicationUser? user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                logger.LogWarning("Login failed: User {Email} not found", dto.Email);
                return (string.Empty, string.Empty, IdentityResult.Failed(new IdentityError { Description = "Invalid credentials" }));
            }

            SignInResult result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                logger.LogWarning("Login failed: Invalid password for user {Email}", dto.Email);
                return (string.Empty, string.Empty, IdentityResult.Failed(new IdentityError { Description = "Invalid credentials" }));
            }

            // Generate JWT
            IList<string> roles = await userManager.GetRolesAsync(user);
            string jwt = GenerateJwtToken(user, roles);
            
            // Generate and store refresh token
            string refreshToken = GenerateRefreshToken();
            RefreshToken refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Revoked = false,
                UserId = user.Id
            };
            
            dbContext.RefreshTokens.Add(refreshTokenEntity);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("User {Email} logged in successfully", dto.Email);
            return (jwt, refreshToken, IdentityResult.Success);
        }

        /// <summary>
        /// Refreshes a JWT using a valid refresh token.
        /// </summary>
        public async Task<(string Jwt, string RefreshToken, bool Success, string? ErrorMessage)> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return (string.Empty, string.Empty, false, "Refresh token is required");
            }

            RefreshToken? tokenEntity = await dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (tokenEntity == null)
            {
                logger.LogWarning("Refresh token not found: {Token}", refreshToken);
                return (string.Empty, string.Empty, false, "Invalid refresh token");
            }

            if (tokenEntity.Revoked || tokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                logger.LogWarning("Refresh token is revoked or expired: {Token}", refreshToken);
                return (string.Empty, string.Empty, false, "Refresh token is invalid or expired");
            }

            ApplicationUser user = tokenEntity.User;
            IList<string> roles = await userManager.GetRolesAsync(user);
            
            // Generate new JWT and refresh token
            string newJwt = GenerateJwtToken(user, roles);
            string newRefreshToken = GenerateRefreshToken();
            
            // Revoke old token and create new one
            tokenEntity.Revoked = true;
            RefreshToken newTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Revoked = false,
                UserId = user.Id
            };
            
            dbContext.RefreshTokens.Add(newTokenEntity);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Refresh token renewed successfully for user {Email}", user.Email);
            return (newJwt, newRefreshToken, true, null);
        }

        /// <summary>
        /// Generates a JWT token for the specified user and roles.
        /// </summary>
        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "dev_secret_key_1234567890");
            string issuer = configuration["Jwt:Issuer"] ?? "Footactique";
            
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim("username", user.UserName ?? user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)))),
                Expires = DateTime.UtcNow.AddHours(12),
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generates a secure refresh token.
        /// </summary>
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        /// <summary>
        /// Gets the current user's profile information.
        /// </summary>
        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            logger.LogInformation("Getting profile for user: {UserId}", userId);
            
            ApplicationUser? user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User not found: {UserId}", userId);
                return null;
            }

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                CreatedAt = DateTime.UtcNow // Utilise la date actuelle comme fallback
            };
        }

        /// <summary>
        /// Updates the current user's profile information.
        /// </summary>
        public async Task<IdentityResult> UpdateUserProfileAsync(string userId, UpdateUserDto dto)
        {
            logger.LogInformation("Updating profile for user: {UserId}", userId);
            
            ApplicationUser? user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User not found: {UserId}", userId);
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            // Verify current password only if updating password or if password is provided
            if (!string.IsNullOrWhiteSpace(dto.NewPassword) || !string.IsNullOrWhiteSpace(dto.CurrentPassword))
            {
                bool isPasswordValid = await userManager.CheckPasswordAsync(user, dto.CurrentPassword);
                if (!isPasswordValid)
                {
                    logger.LogWarning("Invalid current password for user: {UserId}", userId);
                    return IdentityResult.Failed(new IdentityError { Description = "Current password is incorrect" });
                }
            }

            // Update username if provided
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.UserName)
            {
                IdentityResult usernameResult = await userManager.SetUserNameAsync(user, dto.Username);
                if (!usernameResult.Succeeded)
                {
                    logger.LogWarning("Failed to update username for user {UserId}. Errors: {Errors}", 
                        userId, string.Join(", ", usernameResult.Errors.Select(e => e.Description)));
                    return usernameResult;
                }
            }

            // Update email if provided
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                IdentityResult emailResult = await userManager.SetEmailAsync(user, dto.Email);
                if (!emailResult.Succeeded)
                {
                    logger.LogWarning("Failed to update email for user {UserId}. Errors: {Errors}", 
                        userId, string.Join(", ", emailResult.Errors.Select(e => e.Description)));
                    return emailResult;
                }
            }

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                IdentityResult passwordResult = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    logger.LogWarning("Failed to update password for user {UserId}. Errors: {Errors}", 
                        userId, string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
                    return passwordResult;
                }
            }

            logger.LogInformation("Profile updated successfully for user: {UserId}", userId);
            return IdentityResult.Success;
        }

        /// <summary>
        /// Generates a new JWT token for the specified user.
        /// </summary>
        public async Task<string> GenerateNewTokenAsync(string userId)
        {
            logger.LogInformation("Generating new token for user: {UserId}", userId);
            
            ApplicationUser? user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User not found: {UserId}", userId);
                throw new InvalidOperationException("User not found");
            }

            IList<string> roles = await userManager.GetRolesAsync(user);
            string jwt = GenerateJwtToken(user, roles);
            
            logger.LogInformation("New token generated successfully for user: {UserId}", userId);
            return jwt;
        }
    }
} 