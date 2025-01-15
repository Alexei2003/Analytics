using Analytics.Server.Funcs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Tests;

namespace Tests
{
    public class SurveyControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _clientValid;
        private readonly HttpClient _clientNotValid;

        public SurveyControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task GetSurvey_ReturnUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/Survey/1");

            // Act
            var response = await _clientNotValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetSurvey_ReturnSurvey()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/Survey/1");

            // Act
            var response = await _clientValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
