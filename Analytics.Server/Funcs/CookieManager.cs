using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Analytics.Server.Funcs
{
    public class CookiesManager
    {
        public const string SECRET_KEY = "APTd4THGDv&gSa%vG5MsGecwk!!X8p7nN6AaMfyDK7hCWE%hkV*gT8*d&8+EpC*W";
        public const string ISSUER = "EmployeeAnalysisApp";
        public const string AUDIENCE = "Cites";

        public const string TOKEN_KEY = "AuthToken";
        public const string USER_ROLE_KEY = "UserRole";
        public const string EMPLOYEE_ID_KEY = "EmployeeId";
        public const int BASE_TIME_LIFE = 30;

        public struct Roles
        {
            public const string ADMIN = "Admin";
            public const string HR = "HR";
            public const string EMPLOYEE = "Employee";

            public static string[] GetAll()
            {
                string[] res = [ADMIN, HR, EMPLOYEE];

                return res;
            }
        }

        public string UserRole { get; private set; } = "";
        public int EmployeeId { get; private set; } = -1;

        public void GenerateTokenAndSet(HttpResponse response, string login, string roleName, int employeeId, DateTime? timeLife = null)
        {
            var tokenStr = GenerateToken(login, roleName, employeeId ,timeLife);
            SetCookie(response, TOKEN_KEY, tokenStr);
            UserRole = roleName;
            SetCookie(response, USER_ROLE_KEY, roleName, false);
            EmployeeId = employeeId;
            SetCookie(response, EMPLOYEE_ID_KEY, employeeId.ToString(), false);
            return;
        }

        public static string GenerateToken(string login, string roleName, int employeeId, DateTime? timeLife = null)
        {
            if (timeLife == null)
            {
                timeLife = DateTime.Now.AddMinutes(30);
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(USER_ROLE_KEY, roleName),
                new Claim(EMPLOYEE_ID_KEY, employeeId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: ISSUER,
                audience: AUDIENCE,
                expires: timeLife,
                signingCredentials: creds,
                claims: claims
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenStr;
        }

        private static void SetCookie(HttpResponse response, string key, string value, bool httpOnly = true, DateTime? timeLife = null)
        {
            if (timeLife == null)
            {
                timeLife = DateTime.Now.AddMinutes(BASE_TIME_LIFE);
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = httpOnly,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = timeLife
            };

            response.Cookies.Append(key, value, cookieOptions);
        }

        public bool CheckTokenValidity(HttpRequest request, string[] Roles)
        {
            var token = GetCookie(request, TOKEN_KEY);

            if (token == null)
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SECRET_KEY);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = ISSUER,
                    ValidAudience = AUDIENCE,
                    ClockSkew = TimeSpan.Zero,
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var claim = principal.FindFirst(USER_ROLE_KEY);
                if (claim == null)
                {
                    return false;
                }
                UserRole = claim.Value;

                if (!Roles.Contains(UserRole))
                {
                    return false;
                }

                claim = principal.FindFirst(EMPLOYEE_ID_KEY);
                if (claim == null)
                {
                    return false;
                }
                EmployeeId = int.Parse(claim.Value);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
        }

        private static string GetCookie(HttpRequest request, string key)
        {
            if (request.Cookies.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }

        private static void DeleteCookie(HttpResponse response, string key)
        {
            SetCookie(response, key, "", false, DateTime.Now);
        }

        public static void DeleteAllCookies(HttpResponse response)
        {
            DeleteCookie(response, TOKEN_KEY);
            DeleteCookie(response, USER_ROLE_KEY);
            DeleteCookie(response, EMPLOYEE_ID_KEY);
        }
    }
}
