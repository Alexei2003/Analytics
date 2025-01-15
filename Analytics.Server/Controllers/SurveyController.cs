using Analytics.Server.Funcs;
using Analytics.Server.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SurveyController : ControllerBase
    {

        private readonly ILogger<AuthorizationController> _logger;

        public SurveyController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}", Name = "GetSurvey")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSurvey(int id)
        {
            var сookiesManager = new CookiesManager();
            if (!сookiesManager.CheckTokenValidity(Request, CookiesManager.Roles.GetAll()))
            {
                return Unauthorized();
            }

            string sql = @"
                SELECT 
                    s.survey_id,
                    sq.question_id,
                    sr.response_id,
                    s.title,
                    s.image_url,
                    sq.question_text,
                    sr.answer,
                    sr.score,
                    s.description,
                    s.creation_date,
                    s.end_date
                FROM 
                    Surveys s
                LEFT JOIN 
                    Survey_Questions sq ON s.survey_id = sq.survey_id
                LEFT JOIN 
                    Survey_Responses sr ON sq.question_id = sr.question_id
                WHERE 
                    s.survey_id = @survey_id
                ;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@survey_id", id);

                using (var reader = command.ExecuteReader())
                {
                    var survey = new Survey();

                    int surveyId = -1;
                    int questionId = -1;
                    int responseId = -1;

                    Survey.Question question = null;

                    while (reader.Read())
                    {
                        if (surveyId != reader.GetInt32(0))
                        {
                            surveyId = reader.GetInt32(0);

                            survey.Id = surveyId;
                            survey.Title = reader.GetString(3);

                            survey.PhotoUrl = reader.IsDBNull(4) ? null : reader.GetString(4);
                            survey.Description = reader.GetString(8);

                            //survey.CreationData = reader.GetDateTime(9);
                            //survey.EndData = reader.GetDateTime(10);
                        }

                        if (questionId != reader.GetInt32(1))
                        {
                            questionId = reader.GetInt32(1);

                            question = new Survey.Question();
                            question.Id = questionId;
                            question.Text = reader.GetString(5);

                            survey.Questions.Add(question);
                        }

                        if (responseId != reader.GetInt32(2))
                        {
                            responseId = reader.GetInt32(2);

                            var response = new Survey.Question.Answer();

                            response.Id = responseId;
                            response.Text = reader.GetString(6);
                            response.Score = reader.GetInt32(7);

                            question.Answers.Add(response);
                        }
                    }

                    return Ok(survey);
                }
            }
        }

        [HttpPost(Name = "PostSurveyAnswers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSurveyAnswers()
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, CookiesManager.Roles.GetAll()))
            {
                return Unauthorized();
            }

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var responseIds = JsonConvert.DeserializeObject<int[]>(body);

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (int responseId in responseIds)
                    {
                        string sql = @"
                            INSERT INTO Survey_Responses_And_Users (response_id, employee_id)
                            VALUES (@response_id, @employee_id);";

                        using (var command = new NpgsqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@response_id", responseId);
                            command.Parameters.AddWithValue("@employee_id", сookiesManager.EmployeeId);
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit(); // Подтверждаем транзакцию
                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Откатываем транзакцию в случае ошибки
                }
            }

            return BadRequest();
        }
    }
}
