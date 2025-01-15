using Analytics.Server.Funcs;
using Analytics.Server.Objects;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListReportsController : ControllerBase
    {

        private readonly ILogger<AuthorizationController> _logger;

        public ListReportsController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }



        [HttpGet(Name = "GetListReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetListReports([FromQuery] int employeeId)
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, [CookiesManager.Roles.HR, CookiesManager.Roles.ADMIN]))
            {
                return Unauthorized();
            }

            string sql;
            if (employeeId == 0)
            {
                sql = @"
                SELECT
                    r.report_id,
	                r.title,
	                r.date,
	                r.location
                FROM 
	                Reports r;";
            }
            else
            {
                sql = @"
                SELECT
                    r.report_id,
	                r.title,
	                r.date,
	                r.location
                FROM 
	                Reports_And_Users rau
                JOIN 
	                Reports r ON r.report_id = rau.report_id
                WHERE 
	                rau.employee_id = @employeeId;";
            }


            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                if (employeeId != 0)
                {
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                }

                using (var reader = command.ExecuteReader())
                {
                    var listReports = new List<ListReportsElem>();
                    while (reader.Read())
                    {
                        var elem = new ListReportsElem();
                        elem.Id = reader.GetInt32(0);
                        elem.Title = reader.GetString(1);
                        elem.Date = reader.GetDateTime(2);
                        elem.Location = reader.GetString(3);

                        listReports.Add(elem);

                    }
                    return Ok(listReports);
                }
            }
        }
    }
}
