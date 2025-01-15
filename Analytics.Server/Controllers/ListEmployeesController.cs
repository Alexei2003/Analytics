using Analytics.Server.Funcs;
using Analytics.Server.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListEmployeesController : ControllerBase
    {

        private readonly ILogger<AuthorizationController> _logger;

        public ListEmployeesController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetListEmployees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetListEmployees()
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, CookiesManager.Roles.GetAll()))
            {
                return Unauthorized();
            }

            var listEmployees = new List<ListEmployeesElem>();

            string sql = @"
                SELECT 
                    e.employee_id,
                    e.last_name,
                    e.first_name, 
                    e.patronymic,
                    e.position,
                    r.role_name
                FROM 
                    Employees e
                LEFT JOIN 
                    Users u ON e.employee_id = u.employee_id
                LEFT JOIN 
                    Roles r ON u.role_id = r.role_id;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var employee = new ListEmployeesElem
                        {
                            Id = reader.GetInt32(0),
                            LastName = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            Patronymic = reader.GetString(3),
                            Position = reader.GetString(4),
                            Role = reader.GetString(5),
                        };
                        listEmployees.Add(employee);
                    }
                }
            }
            return Ok(listEmployees.ToArray());
        }

        [HttpPut(Name = "ChangeRoles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeRoles()
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, [CookiesManager.Roles.ADMIN]))
            {
                return Unauthorized();
            }

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var newRoles = JsonConvert.DeserializeObject <NewRolesElem[]>(body);

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var newRole in newRoles)
                    {
                        string sql = @"
                            UPDATE Users
                            SET role_id = @roleId
                            WHERE employee_id = @employeeId";

                        using (var command = new NpgsqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@employeeId", newRole.EmployeeId);
                            switch (newRole.Role)
                            {
                                case "Admin":
                                    command.Parameters.AddWithValue("@roleId", 1);
                                    break;
                                case "HR":
                                    command.Parameters.AddWithValue("@roleId", 2);
                                    break;
                                case "Employee":
                                    command.Parameters.AddWithValue("@roleId", 3);
                                    break;
                            }
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
