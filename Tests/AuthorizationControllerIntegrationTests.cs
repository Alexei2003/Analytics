using Analytics.Server.Funcs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using Authorization = Analytics.Server.Objects.Authorization;

namespace Tests
{
    public class AuthorizationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _clientValid;
        private readonly HttpClient _clientNotValid;

        public AuthorizationControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _clientValid = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {

                });
            }).CreateClient();

            _clientValid.DefaultRequestHeaders.Add("Cookie", $"{CookiesManager.TOKEN_KEY}={CookiesManager.GenerateToken("petrov_admin", CookiesManager.Roles.ADMIN, 1)}");

            _clientNotValid = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {

                });
            }).CreateClient();

            _clientNotValid.DefaultRequestHeaders.Add("Cookie", $"{CookiesManager.TOKEN_KEY}={"123"}");

        }

        [Fact]
        public async Task PostLogin_ReturnOk()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "/Authorization");

            var jsonData = new Authorization()
            {
                Login = "petrov_admin",
                Password = "petrov_admin"
            };
            var jsonContent = JsonSerializer.Serialize(jsonData);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _clientValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostLogin_ReturnUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "/Authorization");

            var jsonData = new Authorization()
            {
                Login = "1",
                Password = "2"
            };
            var jsonContent = JsonSerializer.Serialize(jsonData);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _clientNotValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task LogOut_ReturnOk()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Delete, "/Authorization");

            // Act
            var response = await _clientValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LogOut_ReturnUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Delete, "/Authorization");

            // Act
            var response = await _clientNotValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}

