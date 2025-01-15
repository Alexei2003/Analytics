using Analytics.Server.Funcs;
using Analytics.Server.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PostLogin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostLogin()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var authorization = JsonConvert.DeserializeObject<Authorization>(body);

            if (authorization == null)
            {
                return BadRequest();
            }

            //var (a,b) = HashPassword.GetHashPassword(authorization.Password);

            string sql = @"
                SELECT 
                    u.employee_id,
                    u.username,
                    u.password_hash,
                    u.salt,
                    r.role_name
                FROM 
                    Users u
                JOIN 
                    Roles r ON u.role_id = r.role_id
                WHERE 
                    u.username = @username;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", authorization.Login);

                using (var readerSQL = command.ExecuteReader())
                {
                    if (readerSQL.Read())
                    {
                        string passwordHash = readerSQL.GetString(2);
                        byte[] salt = (byte[])readerSQL[3];

                        if (HashPassword.VerifyPassword(authorization.Password, passwordHash, salt))
                        {
                            int employee_id = readerSQL.GetInt32(0);
                            string userName = readerSQL.GetString(1);
                            string roleName = readerSQL.GetString(4);
                            var ñookiesManager = new CookiesManager();
                            ñookiesManager.GenerateTokenAndSet(Response, userName, roleName, employee_id);
                            return Ok();
                        }

                    }
                }
            }
            return Unauthorized();
        }

        [HttpDelete(Name = "LogOut")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LogOut()
        {
            var ñookiesManager = new CookiesManager();

            if (!ñookiesManager.CheckTokenValidity(Request, CookiesManager.Roles.GetAll()))
            {
                return Unauthorized();
            }

            CookiesManager.DeleteAllCookies(Response);

            return Ok();
        }
    }
}
