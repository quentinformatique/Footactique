using Microsoft.EntityFrameworkCore;
using Footactique.Services.Models;

namespace Footactique.Services
{
    /// <summary>
    /// Application database context for EF Core.
    /// </summary>
    public class ApplicationDbContext : DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationship between TeamComposition and PlayerPosition
            modelBuilder.Entity<TeamComposition>()
                .HasMany(tc => tc.Players)
                .WithOne(pp => pp.TeamComposition)
                .HasForeignKey(pp => pp.TeamCompositionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
