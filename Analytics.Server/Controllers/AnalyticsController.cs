using Analytics.Server.Funcs;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;

        public AnalyticsController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }


        [HttpGet(Name = "GetData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetData()
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, [CookiesManager.Roles.HR, CookiesManager.Roles.ADMIN]))
            {
                return Unauthorized();
            }

            string sql = @"
                SELECT 
                    gender,
                    birth_date,
                    work_experience,
                    marital_status
                FROM 
                    Employees";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {

                using (var reader = command.ExecuteReader())
                {
                    var analyticsDataRes = new Objects.Analytics();
                    var maritalStatusChart = new Objects.Analytics.DataChartStr();
                    var now = DateTime.Now;
                    while (reader.Read())
                    {
                        analyticsDataRes.GenderChart.Inc(reader.GetString(0));

                        var birthDate = reader.GetDateTime(1);
                        analyticsDataRes.AgeChart.Inc((now.Year - birthDate.Year).ToString());

                        analyticsDataRes.WorkExperienceChart.Inc(reader.GetInt32(2).ToString());
                        maritalStatusChart.Inc(reader.GetString(3));
                    }

                    var count = 0;
                    var sum = 0;
                    foreach (var item in analyticsDataRes.WorkExperienceChart.Data)
                    {
                        sum += int.Parse(item.Key) * item.Value;
                        count += item.Value;
                    }

                    if (count == 0)
                    {
                        count = 1;
                    }

                    analyticsDataRes.WorkExperienceMean = sum / (float)count;

                    foreach (var item in analyticsDataRes.WorkExperienceChart.Data)
                    {
                        if (int.Parse(item.Key) > analyticsDataRes.WorkExperienceMean)
                        {
                            analyticsDataRes.WorkExperienceMeanChart.Inc("Больше среднего", item.Value);
                        }

                        if (int.Parse(item.Key) < analyticsDataRes.WorkExperienceMean)
                        {
                            analyticsDataRes.WorkExperienceMeanChart.Inc("Меньше среднего", item.Value);
                        }
                    }


                    analyticsDataRes.MaritalStatusChart.Add("В браке", maritalStatusChart.Get("Женатый") + maritalStatusChart.Get("Замужняя"));
                    analyticsDataRes.MaritalStatusChart.Add("Не в браке", maritalStatusChart.Get("Холост") + maritalStatusChart.Get("Незамужняя"));

                    return Ok(analyticsDataRes);
                }
            }
        }
    }
}
