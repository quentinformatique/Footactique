using System.Net.Http.Json;
using Footactique.Api;
using Footactique.Api.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Footactique.Tests
{
    [TestClass]
    public class TeamCompositionsApiIntegrationTests
    {
        private static WebApplicationFactory<Program> _factory;
        private static HttpClient _client;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task GetTeamCompositions_ReturnsOkAndList()
        {
            // Arrange
            // (Assume DB is empty at start)

            // Act
            HttpResponseMessage response = await _client.GetAsync("/api/TeamCompositions");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            List<TeamCompositionDto> result = await response.Content.ReadFromJsonAsync<List<TeamCompositionDto>>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task CreateAndGetTeamComposition_WorksCorrectly()
        {
            // Arrange
            CreateTeamCompositionDto createDto = new CreateTeamCompositionDto
            {
                Name = "IntegrationTest",
                Formation = "4-4-2",
                Players = new List<CreatePlayerPositionDto>
                {
                    new CreatePlayerPositionDto { PlayerName = "Player1", Position = "GK", X = 0.5f, Y = 0.1f },
                    new CreatePlayerPositionDto { PlayerName = "Player2", Position = "DF", X = 0.2f, Y = 0.3f }
                }
            };

            // Act
            HttpResponseMessage postResponse = await _client.PostAsJsonAsync("/api/TeamCompositions", createDto);
            // Assert
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
            TeamCompositionDto created = await postResponse.Content.ReadFromJsonAsync<TeamCompositionDto>();
            Assert.IsNotNull(created);
            Assert.AreEqual("IntegrationTest", created.Name);
            Assert.AreEqual(2, created.Players.Count);

            // Act 2: Get by ID
            HttpResponseMessage getResponse = await _client.GetAsync($"/api/TeamCompositions/{created.Id}");
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            TeamCompositionDto fetched = await getResponse.Content.ReadFromJsonAsync<TeamCompositionDto>();
            Assert.IsNotNull(fetched);
            Assert.AreEqual(created.Id, fetched.Id);
        }

        [TestMethod]
        public async Task UpdateTeamComposition_WorksCorrectly()
        {
            // Arrange: create first
            CreateTeamCompositionDto createDto = new CreateTeamCompositionDto
            {
                Name = "ToUpdate",
                Formation = "4-4-2",
                Players = new List<CreatePlayerPositionDto>()
            };
            HttpResponseMessage postResponse = await _client.PostAsJsonAsync("/api/TeamCompositions", createDto);
            TeamCompositionDto created = await postResponse.Content.ReadFromJsonAsync<TeamCompositionDto>();

            UpdateTeamCompositionDto updateDto = new UpdateTeamCompositionDto
            {
                Name = "UpdatedName",
                Formation = "3-5-2",
                Players = new List<CreatePlayerPositionDto>()
            };

            // Act
            HttpResponseMessage putResponse = await _client.PutAsJsonAsync($"/api/TeamCompositions/{created.Id}", updateDto);

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Act 2: Get and check
            HttpResponseMessage getResponse = await _client.GetAsync($"/api/TeamCompositions/{created.Id}");
            TeamCompositionDto updated = await getResponse.Content.ReadFromJsonAsync<TeamCompositionDto>();
            Assert.AreEqual("UpdatedName", updated.Name);
            Assert.AreEqual("3-5-2", updated.Formation);
        }

        [TestMethod]
        public async Task DeleteTeamComposition_WorksCorrectly()
        {
            // Arrange: create first
            CreateTeamCompositionDto createDto = new CreateTeamCompositionDto
            {
                Name = "ToDelete",
                Formation = "4-4-2",
                Players = new List<CreatePlayerPositionDto>()
            };
            HttpResponseMessage postResponse = await _client.PostAsJsonAsync("/api/TeamCompositions", createDto);
            TeamCompositionDto created = await postResponse.Content.ReadFromJsonAsync<TeamCompositionDto>();

            // Act
            HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/api/TeamCompositions/{created.Id}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Act 2: Get should return 404
            HttpResponseMessage getResponse = await _client.GetAsync($"/api/TeamCompositions/{created.Id}");
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
} 