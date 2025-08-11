using Microsoft.EntityFrameworkCore;
using Footactique.Services.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Footactique.Services
{
    /// <summary>
    /// Application database context for EF Core and Identity.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// Team compositions table.
        /// </summary>
        public DbSet<TeamComposition> TeamCompositions { get; set; }

        /// <summary>
        /// Player positions table.
        /// </summary>
        public DbSet<PlayerPosition> PlayerPositions { get; set; }

        /// <summary>
        /// Refresh tokens table.
        /// </summary>
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // This method should not be used in production
            // The DbContext should be configured through dependency injection in Program.cs
            // This is only for design-time tools like migrations
            if (!optionsBuilder.IsConfigured)
            {
                // For design-time tools, we'll use a simple configuration
                // In production, this should come from appsettings.json
                throw new InvalidOperationException(
                    "DbContext not configured. Ensure it's configured through dependency injection in Program.cs");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationship between TeamComposition and PlayerPosition
            modelBuilder.Entity<TeamComposition>()
                .HasMany(tc => tc.Players)
                .WithOne(pp => pp.TeamComposition)
                .HasForeignKey(pp => pp.TeamCompositionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure default values for new fields
            modelBuilder.Entity<TeamComposition>()
                .Property(tc => tc.IsFavorite)
                .HasDefaultValue(false);

            modelBuilder.Entity<TeamComposition>()
                .Property(tc => tc.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<TeamComposition>()
                .Property(tc => tc.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
