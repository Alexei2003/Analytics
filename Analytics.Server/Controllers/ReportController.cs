using Analytics.Server.Funcs;
using Analytics.Server.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {

        private readonly ILogger<AuthorizationController> _logger;

        public ReportController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }



        [HttpGet("{reportId}", Name = "GetReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReport(int reportId)
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, [CookiesManager.Roles.HR, CookiesManager.Roles.ADMIN]))
            {
                return Unauthorized();
            }

            Report report = null;

            string sql = @"
            SELECT 
                r.report_id,
                r.title,
                r.details,
                r.date,
                r.location,
                ec.category_name
            FROM 
                Reports r
            JOIN 
                Event_Categories ec ON r.category_id = ec.category_id
            WHERE 
	            r.report_id = @reportId;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@reportId", reportId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        report = new Report
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Details = reader.GetString(2),
                            Date = reader.GetDateTime(3),
                            Location = reader.GetString(4),
                            CategoryName = reader.GetString(5)
                        };
                    }
                }
            }

            if (report == null)
            {
                return NotFound();
            }

            sql = @"
            SELECT 
	            e.employee_id,
	            e.last_name,
	            e.first_name,
	            e.patronymic,
	            e.position
            FROM 
	            Reports_And_Users rau
            JOIN 
	            Employees e ON e.employee_id = rau.employee_id
            WHERE 
	            rau.report_id = @reportId;";

            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@reportId", reportId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var employee = new ListEmployeesElem
                        {
                            Id = reader.GetInt32(0),
                            LastName = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            Patronymic = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Position = reader.GetString(4),
                            Role = ""
                        };
                        report.Employees.Add(employee);
                    }
                }
            }

            if (report != null)
            {
                return Ok(report);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost(Name = "PostReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostReport()
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, [CookiesManager.Roles.HR, CookiesManager.Roles.ADMIN]))
            {
                return Unauthorized();
            }

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync(); var report = JsonConvert.DeserializeObject<Report>(body);

            string sql = @"
                INSERT INTO Reports (title, details, date, location, category_id) 
                VALUES (@title, @details, @date, @location, @categoryId) 
                RETURNING report_id;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();

            using var transaction = connection.BeginTransaction();


            int rowsAffected = 0;
            int newReportId;

            using (var command = new NpgsqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@title", report.Title);
                command.Parameters.AddWithValue("@details", report.Details);
                command.Parameters.AddWithValue("@date", report.Date);
                command.Parameters.AddWithValue("@location", report.Location);
                switch (report.CategoryName)
                {
                    case "Training":
                        command.Parameters.AddWithValue("@categoryId", 1);
                        break;
                    case "Workshop":
                        command.Parameters.AddWithValue("@categoryId", 2);
                        break;
                    case "Conference":
                        command.Parameters.AddWithValue("@categoryId", 3);
                        break;
                    case "Webinar":
                        command.Parameters.AddWithValue("@categoryId", 4);
                        break;
                }

                newReportId = (int)command.ExecuteScalar();
            }

            if (report.Employees != null && report.Employees.Count > 0)
            {
                foreach (var employee in report.Employees)
                {
                    string insertEmployeeSql = @"
                        INSERT INTO Reports_And_Users (report_id, employee_id) 
                        VALUES (@reportId, @employeeId);";

                    using (var employeeCommand = new NpgsqlCommand(insertEmployeeSql, connection, transaction))
                    {
                        employeeCommand.Parameters.AddWithValue("@reportId", newReportId);
                        employeeCommand.Parameters.AddWithValue("@employeeId", employee.Id);
                        rowsAffected += employeeCommand.ExecuteNonQuery();
                    }
                }
            }

            transaction.Commit();

            if (rowsAffected > 0)
                return Ok();
            else
                return BadRequest();
        }
    }
}
