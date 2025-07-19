using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Footactique.Services;
using Footactique.Services.Models;
using Footactique.Services.Services;
using Footactique.Contracts.DTOs;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Footactique.Tests
{
    [TestClass]
    public class TeamCompositionServiceTests
    {
        private ApplicationDbContext _context;
        private TeamCompositionService _service;
        private Mock<ILogger<TeamCompositionService>> _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            // Setup in-memory database
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Setup Logger mock
            _loggerMock = new Mock<ILogger<TeamCompositionService>>();

            _service = new TeamCompositionService(_context, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetTeamCompositionsAsync_WithValidUserId_ReturnsUserCompositions()
        {
            // Arrange
            string userId = "test-user-id";
            
            TeamComposition composition = new TeamComposition
            {
                Name = "Test Formation",
                Formation = "4-4-2",
                UserId = userId,
                Players = new List<PlayerPosition>
                {
                    new PlayerPosition { PlayerName = "Player 1", Position = "GK", Number = 1, X = 50, Y = 90 }
                }
            };

            _context.TeamCompositions.Add(composition);
            await _context.SaveChangesAsync();

            // Act
            List<TeamComposition> result = await _service.GetTeamCompositionsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Formation", result[0].Name);
        }

        [TestMethod]
        public async Task GetTeamCompositionAsync_WithValidId_ReturnsComposition()
        {
            // Arrange
            string userId = "test-user-id";
            
            TeamComposition composition = new TeamComposition
            {
                Id = 1,
                Name = "Test Formation",
                Formation = "4-4-2",
                UserId = userId,
                Players = new List<PlayerPosition>
                {
                    new PlayerPosition { PlayerName = "Player 1", Position = "GK", Number = 1, X = 50, Y = 90 }
                }
            };

            _context.TeamCompositions.Add(composition);
            await _context.SaveChangesAsync();

            // Act
            TeamComposition? result = await _service.GetTeamCompositionAsync(userId, 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Formation", result.Name);
        }

        [TestMethod]
        public async Task CreateTeamCompositionAsync_WithValidData_CreatesComposition()
        {
            // Arrange
            string userId = "test-user-id";
            
            TeamComposition composition = new TeamComposition
            {
                Name = "New Formation",
                Formation = "3-5-2",
                UserId = userId,
                Players = new List<PlayerPosition>
                {
                    new PlayerPosition { PlayerName = "Goalkeeper", Position = "GK", Number = 1, X = 50, Y = 90 }
                }
            };

            // Act
            TeamComposition result = await _service.CreateTeamCompositionAsync(userId, composition);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Formation", result.Name);
            Assert.AreEqual(userId, result.UserId);
        }

        [TestMethod]
        public async Task UpdateTeamCompositionAsync_WithValidData_UpdatesComposition()
        {
            // Arrange
            string userId = "test-user-id";
            
            TeamComposition existingComposition = new TeamComposition
            {
                Id = 1,
                Name = "Original Formation",
                Formation = "4-4-2",
                UserId = userId,
                Players = new List<PlayerPosition>
                {
                    new PlayerPosition { PlayerName = "Player 1", Position = "GK", Number = 1, X = 50, Y = 90 }
                }
            };

            _context.TeamCompositions.Add(existingComposition);
            await _context.SaveChangesAsync();

            TeamComposition updatedComposition = new TeamComposition
            {
                Id = 1,
                Name = "Updated Formation",
                Formation = "3-5-2",
                UserId = userId,
                Players = new List<PlayerPosition>
                {
                    new PlayerPosition { PlayerName = "Updated Player", Position = "GK", Number = 1, X = 50, Y = 90 }
                }
            };

            // Act
            bool result = await _service.UpdateTeamCompositionAsync(userId, 1, updatedComposition);

            // Assert
            Assert.IsTrue(result);
            TeamComposition? savedComposition = await _context.TeamCompositions.FindAsync(1);
            Assert.IsNotNull(savedComposition);
            Assert.AreEqual("Updated Formation", savedComposition.Name);
        }

        [TestMethod]
        public async Task DeleteTeamCompositionAsync_WithValidId_DeletesComposition()
        {
            // Arrange
            string userId = "test-user-id";
            
            TeamComposition composition = new TeamComposition
            {
                Id = 1,
                Name = "To Delete",
                Formation = "4-4-2",
                UserId = userId,
                Players = new List<PlayerPosition>
                {
                    new PlayerPosition { PlayerName = "Player 1", Position = "GK", Number = 1, X = 50, Y = 90 }
                }
            };

            _context.TeamCompositions.Add(composition);
            await _context.SaveChangesAsync();

            // Act
            bool result = await _service.DeleteTeamCompositionAsync(userId, 1);

            // Assert
            Assert.IsTrue(result);
            TeamComposition? deletedComposition = await _context.TeamCompositions.FindAsync(1);
            Assert.IsNull(deletedComposition);
        }

        [TestMethod]
        public async Task GetTeamCompositionAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            string userId = "test-user-id";

            // Act
            TeamComposition? result = await _service.GetTeamCompositionAsync(userId, 999);

            // Assert
            Assert.IsNull(result);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }
    }
} 