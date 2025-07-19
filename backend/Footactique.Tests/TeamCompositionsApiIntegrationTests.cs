using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Footactique.Api;
using Footactique.Services;
using Footactique.Contracts.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Footactique.Tests
{
    [TestClass]
    public class TeamCompositionsApiIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [TestInitialize]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    ServiceDescriptor? descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(ApplicationDbContext));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database for testing
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb_TeamCompositions");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public async Task GetTeamCompositions_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            HttpResponseMessage response = await _client.GetAsync("/api/TeamCompositions");

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateTeamComposition_WithValidData_ReturnsCreated()
        {
            // Arrange - Register and login first
            RegisterDto registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "TestPassword123!"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            LoginDto loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "TestPassword123!"
            };
            HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            string loginContent = await loginResponse.Content.ReadAsStringAsync();
            
            // Extract JWT token from login response
            JsonDocument loginDoc = JsonDocument.Parse(loginContent);
            string jwtToken = loginDoc.RootElement.GetProperty("token").GetString() ?? string.Empty;

            // Set authorization header
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            CreateTeamCompositionDto createDto = new CreateTeamCompositionDto
            {
                Name = "Test Formation",
                Formation = "4-4-2",
                Players = new List<CreatePlayerPositionDto>
                {
                    new CreatePlayerPositionDto
                    {
                        PlayerName = "Goalkeeper",
                        Position = "GK",
                        Number = 1,
                        X = 50,
                        Y = 90
                    },
                    new CreatePlayerPositionDto
                    {
                        PlayerName = "Defender",
                        Position = "DEF",
                        Number = 4,
                        X = 50,
                        Y = 70
                    }
                }
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/TeamCompositions", createDto);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("Test Formation"));
            Assert.IsTrue(content.Contains("4-4-2"));
        }

        [TestMethod]
        public async Task GetTeamCompositions_WithAuth_ReturnsUserCompositions()
        {
            // Arrange - Register and login first
            RegisterDto registerDto = new RegisterDto
            {
                Email = "list@example.com",
                Password = "TestPassword123!"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            LoginDto loginDto = new LoginDto
            {
                Email = "list@example.com",
                Password = "TestPassword123!"
            };
            HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            string loginContent = await loginResponse.Content.ReadAsStringAsync();
            
            // Extract JWT token from login response
            JsonDocument loginDoc = JsonDocument.Parse(loginContent);
            string jwtToken = loginDoc.RootElement.GetProperty("token").GetString() ?? string.Empty;

            // Set authorization header
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            // Create a composition first
            CreateTeamCompositionDto createDto = new CreateTeamCompositionDto
            {
                Name = "List Test Formation",
                Formation = "3-5-2",
                Players = new List<CreatePlayerPositionDto>
                {
                    new CreatePlayerPositionDto
                    {
                        PlayerName = "Goalkeeper",
                        Position = "GK",
                        Number = 1,
                        X = 50,
                        Y = 90
                    }
                }
            };
            await _client.PostAsJsonAsync("/api/TeamCompositions", createDto);

            // Act
            HttpResponseMessage response = await _client.GetAsync("/api/TeamCompositions");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("List Test Formation"));
        }

        [TestMethod]
        public async Task UpdateTeamComposition_WithValidData_ReturnsSuccess()
        {
            // Arrange - Register and login first
            RegisterDto registerDto = new RegisterDto
            {
                Email = "update@example.com",
                Password = "TestPassword123!"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            LoginDto loginDto = new LoginDto
            {
                Email = "update@example.com",
                Password = "TestPassword123!"
            };
            HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            string loginContent = await loginResponse.Content.ReadAsStringAsync();
            
            // Extract JWT token from login response
            JsonDocument loginDoc = JsonDocument.Parse(loginContent);
            string jwtToken = loginDoc.RootElement.GetProperty("token").GetString() ?? string.Empty;

            // Set authorization header
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            // Create a composition first
            CreateTeamCompositionDto createDto = new CreateTeamCompositionDto
            {
                Name = "Original Name",
                Formation = "4-4-2",
                Players = new List<CreatePlayerPositionDto>
                {
                    new CreatePlayerPositionDto
                    {
                        PlayerName = "Goalkeeper",
                        Position = "GK",
                        Number = 1,
                        X = 50,
                        Y = 90
                    }
                }
            };
            HttpResponseMessage createResponse = await _client.PostAsJsonAsync("/api/TeamCompositions", createDto);
            string createContent = await createResponse.Content.ReadAsStringAsync();
            
            // Extract the created composition ID
            JsonDocument createDoc = JsonDocument.Parse(createContent);
            int compositionId = createDoc.RootElement.GetProperty("id").GetInt32();

            UpdateTeamCompositionDto updateDto = new UpdateTeamCompositionDto
            {
                Name = "Updated Name",
                Formation = "3-5-2",
                Players = new List<CreatePlayerPositionDto>
                {
                    new CreatePlayerPositionDto
                    {
                        PlayerName = "Updated Goalkeeper",
                        Position = "GK",
                        Number = 1,
                        X = 50,
                        Y = 90
                    }
                }
            };

            // Act
            HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/TeamCompositions/{compositionId}", updateDto);

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteTeamComposition_WithValidId_ReturnsSuccess()
        {
            // Arrange - Register and login first
            RegisterDto registerDto = new RegisterDto
            {
                Email = "delete@example.com",
                Password = "TestPassword123!"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            LoginDto loginDto = new LoginDto
            {
                Email = "delete@example.com",
                Password = "TestPassword123!"
            };
            HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            string loginContent = await loginResponse.Content.ReadAsStringAsync();
            
            // Extract JWT token from login response
            JsonDocument loginDoc = JsonDocument.Parse(loginContent);
            string jwtToken = loginDoc.RootElement.GetProperty("token").GetString() ?? string.Empty;

            // Set authorization header
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            // Create a composition first
            CreateTeamCompositionDto createDto = new CreateTeamCompositionDto
            {
                Name = "To Delete",
                Formation = "4-4-2",
                Players = new List<CreatePlayerPositionDto>
                {
                    new CreatePlayerPositionDto
                    {
                        PlayerName = "Goalkeeper",
                        Position = "GK",
                        Number = 1,
                        X = 50,
                        Y = 90
                    }
                }
            };
            HttpResponseMessage createResponse = await _client.PostAsJsonAsync("/api/TeamCompositions", createDto);
            string createContent = await createResponse.Content.ReadAsStringAsync();
            
            // Extract the created composition ID
            JsonDocument createDoc = JsonDocument.Parse(createContent);
            int compositionId = createDoc.RootElement.GetProperty("id").GetInt32();

            // Act
            HttpResponseMessage response = await _client.DeleteAsync($"/api/TeamCompositions/{compositionId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
} 