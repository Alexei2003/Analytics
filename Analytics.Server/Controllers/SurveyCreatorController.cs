using Analytics.Server.Funcs;
using Analytics.Server.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SurveyCreatorController : ControllerBase
    {

        private readonly ILogger<AuthorizationController> _logger;

        public SurveyCreatorController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PostCustomSurvey")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostCustomSurvey()
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, [CookiesManager.Roles.HR]))
            {
                return Unauthorized();
            }

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var survey = JsonConvert.DeserializeObject<Survey>(body);

            SurveyCreator.InsertSurvey(survey);

            return Ok();
        }

        [HttpPost("auto", Name = "PostNewAutoSurvey")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostNewAutoSurvey()
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, [CookiesManager.Roles.HR]))
            {
                return Unauthorized();
            }

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var endDate = JsonConvert.DeserializeObject<DateTime>(body);

            var survey = SurveyCreator.GenerateAutoSurvey(endDate);

            SurveyCreator.InsertSurvey(survey);

            return Ok();
        }
    }
}
