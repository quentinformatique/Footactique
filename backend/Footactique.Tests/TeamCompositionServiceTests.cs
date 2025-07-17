using Footactique.Services;
using Footactique.Services.Models;
using Footactique.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Footactique.Tests
{
    [TestClass]
    public class TeamCompositionServiceTests
    {
        private TeamCompositionService CreateServiceWithInMemoryDb(out ApplicationDbContext dbContext)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            dbContext = new ApplicationDbContext(options);
            var logger = new Mock<ILogger<TeamCompositionService>>().Object;
            return new TeamCompositionService(dbContext, logger);
        }

        [TestMethod]
        public async Task GetTeamCompositionsAsync_ReturnsOnlyUserCompositions()
        {
            // Arrange
            TeamCompositionService service = CreateServiceWithInMemoryDb(out ApplicationDbContext dbContext);
            dbContext.TeamCompositions.AddRange(
                new TeamComposition { Name = "A", Formation = "4-4-2", UserId = "user1", Players = new List<PlayerPosition>() },
                new TeamComposition { Name = "B", Formation = "4-3-3", UserId = "user2", Players = new List<PlayerPosition>() }
            );
            await dbContext.SaveChangesAsync();

            // Act
            List<TeamComposition> result = await service.GetTeamCompositionsAsync("user1");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("A", result[0].Name);
        }

        [TestMethod]
        public async Task GetTeamCompositionAsync_ReturnsNullIfNotFound()
        {
            // Arrange
            TeamCompositionService service = CreateServiceWithInMemoryDb(out _);

            // Act
            TeamComposition result = await service.GetTeamCompositionAsync("user1", 999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreateTeamCompositionAsync_SavesAndReturnsComposition()
        {
            // Arrange
            TeamCompositionService service = CreateServiceWithInMemoryDb(out ApplicationDbContext dbContext);
            TeamComposition comp = new TeamComposition { Name = "Test", Formation = "4-4-2", Players = new List<PlayerPosition>() };

            // Act
            TeamComposition result = await service.CreateTeamCompositionAsync("user1", comp);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("user1", result.UserId);
            Assert.AreEqual(1, dbContext.TeamCompositions.Count());
        }

        [TestMethod]
        public async Task UpdateTeamCompositionAsync_UpdatesIfExists()
        {
            // Arrange
            TeamCompositionService service = CreateServiceWithInMemoryDb(out ApplicationDbContext dbContext);
            TeamComposition comp = new TeamComposition { Name = "Old", Formation = "4-4-2", UserId = "user1", Players = new List<PlayerPosition>() };
            dbContext.TeamCompositions.Add(comp);
            await dbContext.SaveChangesAsync();
            TeamComposition updated = new TeamComposition { Name = "New", Formation = "3-5-2", Players = new List<PlayerPosition>() };

            // Act
            bool result = await service.UpdateTeamCompositionAsync("user1", comp.Id, updated);

            // Assert
            Assert.IsTrue(result);
            TeamComposition reloaded = dbContext.TeamCompositions.First(x => x.Id == comp.Id);
            Assert.AreEqual("New", reloaded.Name);
            Assert.AreEqual("3-5-2", reloaded.Formation);
        }

        [TestMethod]
        public async Task UpdateTeamCompositionAsync_ReturnsFalseIfNotFound()
        {
            // Arrange
            TeamCompositionService service = CreateServiceWithInMemoryDb(out _);
            TeamComposition updated = new TeamComposition { Name = "New", Formation = "3-5-2", Players = new List<PlayerPosition>() };

            // Act
            bool result = await service.UpdateTeamCompositionAsync("user1", 999, updated);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteTeamCompositionAsync_DeletesIfExists()
        {
            // Arrange
            TeamCompositionService service = CreateServiceWithInMemoryDb(out ApplicationDbContext dbContext);
            TeamComposition comp = new TeamComposition { Name = "ToDelete", Formation = "4-4-2", UserId = "user1", Players = new List<PlayerPosition>() };
            dbContext.TeamCompositions.Add(comp);
            await dbContext.SaveChangesAsync();

            // Act
            bool result = await service.DeleteTeamCompositionAsync("user1", comp.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, dbContext.TeamCompositions.Count());
        }

        [TestMethod]
        public async Task DeleteTeamCompositionAsync_ReturnsFalseIfNotFound()
        {
            // Arrange
            TeamCompositionService service = CreateServiceWithInMemoryDb(out _);

            // Act
            bool result = await service.DeleteTeamCompositionAsync("user1", 999);

            // Assert
            Assert.IsFalse(result);
        }
    }
} 