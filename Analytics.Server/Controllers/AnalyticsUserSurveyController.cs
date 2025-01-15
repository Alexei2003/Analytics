using Analytics.Server.Funcs;
using Analytics.Server.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Npgsql;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyticsUserSurveyController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;

        public AnalyticsUserSurveyController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }


        [HttpGet("all",Name = "GetAnalyticsAllUserSurvey")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAnalyticsAllUserSurvey([FromQuery] int surveyId)
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, [CookiesManager.Roles.HR, CookiesManager.Roles.ADMIN]))
            {
                return Unauthorized();
            }

            string sql = @"
                SELECT
	                sq.question_text,
	                sr.answer
                FROM
	                Surveys s
                JOIN
	                Survey_Questions sq ON sq.survey_id = s.survey_id
                JOIN
	                Survey_Responses sr ON sr.question_id = sq.question_id
                JOIN
	                Survey_Responses_And_Users sray ON sray.response_id = sr.response_id
                WHERE
	                s.survey_id = @surveyId;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@surveyId", surveyId);

                using (var readerSql = command.ExecuteReader())
                {
                    var dictAnswers = new Dictionary<string, Objects.Analytics.DataChartStr>();
                    while (readerSql.Read())
                    {
                        var questionText = readerSql.GetString(0);
                        var answer = readerSql.GetString(1);

                        dictAnswers.TryAdd(questionText, new Objects.Analytics.DataChartStr());

                        dictAnswers[questionText].Inc(answer);   
                    }
                    return Ok(dictAnswers);
                }
            }
        }

        [HttpGet("user", Name = "GetAnalyticsUserSurvey")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAnalyticsUserSurvey([FromQuery] int surveyId, [FromQuery] int employeeId)
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, [CookiesManager.Roles.HR, CookiesManager.Roles.ADMIN]))
            {
                return Unauthorized();
            }

            string sql = @"
                SELECT
	                sq.question_text,
	                sr.answer,
                    sr.score
                FROM
	                Surveys s
                JOIN
	                Survey_Questions sq ON sq.survey_id = s.survey_id
                JOIN
	                Survey_Responses sr ON sr.question_id = sq.question_id
                JOIN
	                Survey_Responses_And_Users sray ON sray.response_id = sr.response_id
                WHERE
	                s.survey_id = @surveyId AND sray.employee_id = @employeeId;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@surveyId", surveyId);
                command.Parameters.AddWithValue("@employeeId", employeeId);
                using (var readerSql = command.ExecuteReader())
                {
                    var listAnswers = new UserAnswers();
                    while (readerSql.Read())
                    {
                        var questionText = readerSql.GetString(0);
                        var answer = readerSql.GetString(1);
                        var score = readerSql.GetInt32(2);

                        listAnswers.Score += score;
                        listAnswers.Answers.Add(new UserAnswers.UserAnswer
                        {
                            Question = questionText,
                            Answer = answer,
                        });
                    }
                    return Ok(listAnswers);
                }
            }
        }
    }
}
