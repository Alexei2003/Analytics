using Analytics.Server.Funcs;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using ListEmployeesElem = Analytics.Server.Objects.ListEmployeesElem;

namespace Tests
{
    public class ListEmployeesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _clientValid;
        private readonly HttpClient _clientNotValid;

        public ListEmployeesControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task GetListEmployees_ReturnUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/ListEmployees");

            // Act
            var response = await _clientNotValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetListEmployees_ReturnListEmployees()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/ListEmployees");

            // Act
            var response = await _clientValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();

            var listEmployeesData = JsonConvert.DeserializeObject<List<ListEmployeesElem>>(responseBody);
            Assert.NotNull(listEmployeesData);

            Assert.Equal("Сидорова", listEmployeesData[0].LastName);
        }
    }
}
