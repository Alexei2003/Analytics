using Analytics.Server.Funcs;
using Analytics.Server.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListSurveysController : ControllerBase
    {

        private readonly ILogger<AuthorizationController> _logger;

        public ListSurveysController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetListSurveys")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetListSurveys([FromQuery] int type)
        {

            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, CookiesManager.Roles.GetAll()))
            {
                return Unauthorized();
            }

            var listSurveys = new List<ListSurveysElem>();

            string sql = @"
                SELECT 
                    s.survey_id,
                    s.title,
                    s.creation_date, 
                    s.end_date,
                    srau.employee_id
                FROM 
                    Surveys s
                LEFT JOIN 
                    Survey_Questions sq ON s.survey_id = sq.survey_id
                LEFT JOIN 
                    Survey_Responses sr ON sq.question_id = sr.question_id
                LEFT JOIN 
                    Survey_Responses_And_Users srau ON sr.response_id = srau.response_id;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {

                using (var readerSql = command.ExecuteReader())
                {
                    var checkedSurveyId = new List<int>();
                    var removeSurveyId = new List<int>();
                    while (readerSql.Read())
                    {
                        if (removeSurveyId.Contains(readerSql.GetInt32(0)))
                        {
                            continue;
                        }

                        var survey = new ListSurveysElem
                        {
                            Id = readerSql.GetInt32(0),
                            Title = readerSql.GetString(1),
                            CreationData = readerSql.GetDateTime(2),
                            EndData = readerSql.GetDateTime(3),
                        };

                        switch (type)
                        {
                            case -1:
                                if (readerSql.IsDBNull(4) || readerSql.GetInt32(4) != сookiesManager.EmployeeId)
                                {
                                    if (!checkedSurveyId.Contains(survey.Id))
                                    {
                                        listSurveys.Add(survey);
                                        checkedSurveyId.Add(survey.Id);
                                    }
                                }
                                else
                                {
                                    removeSurveyId.Add(survey.Id);
                                }
                                break;
                            case 0:
                                if (!checkedSurveyId.Contains(survey.Id))
                                {
                                    listSurveys.Add(survey);
                                    checkedSurveyId.Add(survey.Id);
                                }
                                break;
                            default:
                                if (!readerSql.IsDBNull(4) && readerSql.GetInt32(4) == type)
                                {
                                    if (!checkedSurveyId.Contains(survey.Id))
                                    {
                                        listSurveys.Add(survey);
                                        checkedSurveyId.Add(survey.Id);
                                    }
                                }
                                break;
                        }
                    }

                    for (var i = 0; i < listSurveys.Count; i++)
                    {
                        if (removeSurveyId.Contains(listSurveys[i].Id))
                        {
                            listSurveys.RemoveAt(i);
                        }
                    }
                }
            }

            return Ok(listSurveys);
        }
    }
}
