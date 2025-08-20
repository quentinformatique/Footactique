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
    public class AuthApiIntegrationTests
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
                        options.UseInMemoryDatabase("TestDb_Auth");
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
        public async Task Register_ValidUser_ReturnsSuccess()
        {
            // Arrange
            RegisterDto registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "TestPassword123!"
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("User registered successfully"));
        }

        [TestMethod]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            RegisterDto registerDto = new RegisterDto
            {
                Email = "duplicate@example.com",
                Username = "testuser",
                Password = "TestPassword123!"
            };

            // Register the user first time
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Act - Try to register the same email again
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsJwtToken()
        {
            // Arrange
            RegisterDto registerDto = new RegisterDto
            {
                Email = "login@example.com",
                Username = "testuser",
                Password = "TestPassword123!"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            LoginDto loginDto = new LoginDto
            {
                Email = "login@example.com",
                Password = "TestPassword123!"
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("token"));
            Assert.IsTrue(content.Contains("refreshToken"));
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            LoginDto loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "WrongPassword123!"
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task RefreshToken_ValidToken_ReturnsNewTokens()
        {
            // Arrange
            RegisterDto registerDto = new RegisterDto
            {
                Email = "refresh@example.com",
                Username = "testuser",
                Password = "TestPassword123!"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            LoginDto loginDto = new LoginDto
            {
                Email = "refresh@example.com",
                Password = "TestPassword123!"
            };
            HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            string loginContent = await loginResponse.Content.ReadAsStringAsync();
            
            // Extract refresh token from login response
            JsonDocument loginDoc = JsonDocument.Parse(loginContent);
            string refreshToken = loginDoc.RootElement.GetProperty("refreshToken").GetString() ?? string.Empty;

            RefreshTokenRequestDto refreshDto = new RefreshTokenRequestDto
            {
                RefreshToken = refreshToken
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshDto);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("token"));
            Assert.IsTrue(content.Contains("refreshToken"));
        }
    }
} 