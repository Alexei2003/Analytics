using Analytics.Server.Funcs;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using Employee = Analytics.Server.Objects.Employee;
using ListSkillsElem = Analytics.Server.Objects.ListSkillsElem;

namespace Tests
{
    public class EmployeeControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _clientValid;
        private readonly HttpClient _clientNotValid;

        public EmployeeControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task GetEmployee_ReturnUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/Employee/1");

            // Act
            var response = await _clientNotValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetEmployee_ReturnNotFound()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/Employee/-1");

            // Act
            var response = await _clientValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetEmployee_ReturnEmployee()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/Employee/1");

            // Act
            var response = await _clientValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();

            var employeeData = JsonConvert.DeserializeObject<Employee>(responseBody);
            Assert.NotNull(employeeData);

            Assert.Equal("Петров", employeeData.LastName);
        }

        public async Task GetEmployeeSkills_ReturnUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/Employee/1/skills");

            // Act
            var response = await _clientNotValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task GetEmployeeSkills_ReturnNotFound()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/Employee/-1/skills");

            // Act
            var response = await _clientValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();

            var listSkillsData = JsonConvert.DeserializeObject<List<ListSkillsElem>>(responseBody);
            Assert.NotNull(listSkillsData);
            Assert.Equal(0, listSkillsData.Count());
        }

        [Fact]
        public async Task GetEmployeeSkills_ReturnSkills()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/Employee/1/skills");

            // Act
            var response = await _clientValid.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();

            var listSkillsData = JsonConvert.DeserializeObject<List<ListSkillsElem>>(responseBody);
            Assert.NotNull(listSkillsData);

            Assert.Equal("JavaScript Programming", listSkillsData[0].Name);
        }

    }
}
