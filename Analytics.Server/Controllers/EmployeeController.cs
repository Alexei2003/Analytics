using Analytics.Server.Funcs;
using Analytics.Server.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace Analytics.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {

        private readonly ILogger<AuthorizationController> _logger;

        public EmployeeController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }



        [HttpGet("{id}", Name = "GetEmployee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, CookiesManager.Roles.GetAll()))
            {
                return Unauthorized();
            }

            string sql = @"
                SELECT 
                    *
                FROM 
                    Employees
                WHERE 
                    employee_id = @employeeId;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@employeeId", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Employee employee;
                        if (сookiesManager.UserRole == CookiesManager.Roles.HR || сookiesManager.UserRole == CookiesManager.Roles.ADMIN || сookiesManager.EmployeeId == id)
                        {
                            employee = new Employee
                            {
                                Id = reader.GetInt32(0),
                                LastName = reader.GetString(1),
                                FirstName = reader.GetString(2),
                                Patronymic = reader.IsDBNull(3) ? null : reader.GetString(3),
                                BirthDate = reader.GetDateTime(4),
                                PassportSeries = reader.GetString(5),
                                PassportNumber = reader.GetString(6),
                                PassportIssued = reader.GetString(7),
                                PassportIssuedDate = reader.GetDateTime(8),
                                Citizenship = reader.GetString(9),
                                Address = reader.IsDBNull(10) ? null : reader.GetString(10),
                                Email = reader.GetString(11),
                                Position = reader.GetString(12),
                                WorkExperience = reader.GetInt32(13),
                                MaritalStatus = reader.GetString(14),
                                MonthlyIncome = reader.GetDecimal(15),
                                Gender = reader.GetString(16)
                            };
                        }
                        else
                        {
                            employee = new Employee
                            {
                                Id = reader.GetInt32(0),
                                LastName = reader.GetString(1),
                                FirstName = reader.GetString(2),
                                Patronymic = reader.IsDBNull(3) ? null : reader.GetString(3),
                                BirthDate = reader.GetDateTime(4),
                                Email = reader.GetString(11),
                                Position = reader.GetString(12),
                                WorkExperience = reader.GetInt32(13),
                            };
                        }

                        return Ok(employee);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        [HttpPut("{id}", Name = "ChangeEmployee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeEmployee(int id)
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, CookiesManager.Roles.GetAll()))
            {
                return Unauthorized();
            }

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonConvert.DeserializeObject<Employee>(body);

            string sql = @"
                UPDATE Employees
                SET 
                    last_name = @lastName,
                    first_name = @firstName,
                    patronymic = @patronymic,
                    birth_date = @birthDate,
                    passport_series = @passportSeries,
                    passport_number = @passportNumber,
                    passport_issued = @passportIssued,
                    passport_issue_date = @passportIssueDate,
                    citizenship = @citizenship,
                    address = @address,
                    email = @email,
                    position = @position,
                    work_experience = @workExperience,
                    marital_status = @maritalStatus,
                    monthly_income = @monthlyIncome,
                    gender = @gender
                WHERE employee_id = @employeeId;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@employeeId", id);

                command.Parameters.AddWithValue("@lastName", employee.LastName);
                command.Parameters.AddWithValue("@firstName", employee.FirstName);
                command.Parameters.AddWithValue("@patronymic", (object)employee.Patronymic ?? DBNull.Value);
                command.Parameters.AddWithValue("@birthDate", employee.BirthDate);
                command.Parameters.AddWithValue("@passportSeries", employee.PassportSeries);
                command.Parameters.AddWithValue("@passportNumber", employee.PassportNumber);
                command.Parameters.AddWithValue("@passportIssued", employee.PassportIssued);
                command.Parameters.AddWithValue("@passportIssueDate", employee.PassportIssuedDate);
                command.Parameters.AddWithValue("@citizenship", employee.Citizenship);
                command.Parameters.AddWithValue("@address", (object)employee.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@email", employee.Email);
                command.Parameters.AddWithValue("@phoneNumber", (object)employee.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@photoUrl", (object)employee.PhotoUrl ?? DBNull.Value);
                command.Parameters.AddWithValue("@position", employee.Position);
                command.Parameters.AddWithValue("@workExperience", employee.WorkExperience);
                command.Parameters.AddWithValue("@maritalStatus", employee.MaritalStatus);
                command.Parameters.AddWithValue("@monthlyIncome", employee.MonthlyIncome);
                command.Parameters.AddWithValue("@gender", employee.Gender);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                    return Ok();
                else
                    return NotFound();
            }
        }

        [HttpGet("{id}/skills", Name = "GetEmployeeSkills")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetEmployeeSkills(int id)
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, CookiesManager.Roles.GetAll()))
            {
                return Unauthorized();
            }

            string sql =  @"
                SELECT 
                    skill_id, 
                    employee_id, 
                    skill_name, 
                    proficiency_level 
                FROM 
                    Skills 
                WHERE 
                    employee_id = @employeeId;";

            using var connection = new NpgsqlConnection(DataBase.connString);
            connection.Open();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@employeeId", id);

                var skills = new List<ListSkillsElem>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var skill = new ListSkillsElem
                        {
                            Id = reader.GetInt32(0),
                            EmployeeId = reader.GetInt32(1),
                            Name = reader.GetString(2),
                            ProficiencyLevel = reader.GetInt32(3)
                        };
                        skills.Add(skill);
                    }
                    return Ok(skills);
                }
            }
        }


        [HttpPut("{id}/skills", Name = "ChangeEmployeeSkills")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangeEmployeeSkills(int id)
        {
            var сookiesManager = new CookiesManager();

            if (!сookiesManager.CheckTokenValidity(Request, CookiesManager.Roles.GetAll()))
            {
                return Unauthorized();
            }

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var jsonNode = JsonNode.Parse(body);
            int[] delSkillsIds;
            try
            {
                if (jsonNode["delSkills"] is JsonArray delSkillsArray)
                {
                    delSkillsIds = new int[delSkillsArray.Count];
                    for (int i = 0; i < delSkillsArray.Count; i++)
                    {
                        if (delSkillsArray[i] is JsonValue skillIdValue && skillIdValue.TryGetValue(out int skillId))
                        {
                            delSkillsIds[i] = skillId;
                        }
                    }
                }
                else
                {
                    delSkillsIds = null; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing delSkills: {ex.Message}");
                delSkillsIds = null; 
            }

            ListSkillsElem[] newSkills;
            try
            {
                if (jsonNode["newSkills"] is JsonArray newSkillsArray)
                {
                    newSkills = new ListSkillsElem[newSkillsArray.Count];
                    for (int i = 0; i < newSkillsArray.Count; i++)
                    {
                        if (newSkillsArray[i] is JsonObject skillObject)
                        {
                            var skillId = skillObject["id"]?.ToString();
                            var employeeId = skillObject["employeeId"]?.ToString();
                            var name = skillObject["name"]?.ToString();
                            var proficiencyLevel = skillObject["proficiencyLevel"]?.ToString();

                            newSkills[i] = new ListSkillsElem
                            {
                                Id = int.Parse(skillId),
                                EmployeeId = int.Parse(employeeId),
                                Name = name,
                                ProficiencyLevel = int.Parse(proficiencyLevel)>10? 10: int.Parse(proficiencyLevel)
                            };
                        }
                    }
                }
                else
                {
                    newSkills = null; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing newSkills: {ex.Message}");
                newSkills = null;
            }

            // Удаление указанных навыков
            if (delSkillsIds != null && delSkillsIds.Length > 0)
            {
                string deleteSql = @"
                    DELETE FROM 
                        Skills 
                    WHERE 
                        skill_id = ANY(@SkillIds) AND employee_id = @EmployeeId;";
                using var connection = new NpgsqlConnection(DataBase.connString);
                connection.Open();
                using (var deleteCommand = new NpgsqlCommand(deleteSql, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@SkillIds", delSkillsIds);
                    deleteCommand.Parameters.AddWithValue("@EmployeeId", id);
                    deleteCommand.ExecuteNonQuery(); 
                }
            }

            // Добавление новых навыков
            string insertSql = @"
                INSERT INTO Skills (employee_id, skill_name, proficiency_level)
                VALUES (@EmployeeId, @SkillName, @ProficiencyLevel);";

            if (newSkills != null)
            {
                foreach (var skill in newSkills)
                {
                    using var connection = new NpgsqlConnection(DataBase.connString);
                    connection.Open();
                    using (var insertCommand = new NpgsqlCommand(insertSql, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@EmployeeId", id);
                        insertCommand.Parameters.AddWithValue("@SkillName", skill.Name);
                        insertCommand.Parameters.AddWithValue("@ProficiencyLevel", skill.ProficiencyLevel);

                        insertCommand.ExecuteNonQuery(); 
                    }
                }
            }

            return Ok();
        }
    }
}
