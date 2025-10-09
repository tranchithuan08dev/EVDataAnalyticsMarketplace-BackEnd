using EV.AdminService.API.ConfigurationModels;
using EV.AdminService.API.Models.DataModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EV.AdminService.API.Helpers
{
    public class JwtHelper
    {
        private readonly JwtSettings _settings;

        public JwtHelper(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateToken(User user, IEnumerable<string>? roles = null)
        {
            var now = DateTime.UtcNow;
            var jti = Guid.NewGuid().ToString("N");

            var claims = new List<Claim>
{
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString() ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.DisplayName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (roles != null)
            {
                foreach (var r in roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct())
                {
                    claims.Add(new Claim(ClaimTypes.Role, r));
                    claims.Add(new Claim("role", r));
                }

                var rolesArray = roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToArray();
                if (rolesArray.Length > 0)
                {
                    claims.Add(new Claim("roles", JsonConvert.SerializeObject(rolesArray), JsonClaimValueTypes.JsonArray));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                notBefore: now,
                claims: claims,
                expires: now.AddMinutes(_settings.ExpirationInMinutes),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Generated JWT: {jwt}");
            return jwt;
        }

        public (string? Jti, DateTime? ExpiresUtc) ReadJtiAndExpiry(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var exp = jwt.Payload.Expiration;
                DateTime? expUtc = exp.HasValue
                    ? DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime
                    : null;
                return (jti, expUtc);
            }
            catch
            {
                return (null, null);
            }
        }

        public static ClaimsPrincipal? GetPrincipalFromToken(string token, JwtSettings settings, out SecurityToken? validatedToken)
        {
            validatedToken = null;
            if (string.IsNullOrEmpty(token)) return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrEmpty(settings.Issuer),
                ValidIssuer = settings.Issuer,

                ValidateAudience = !string.IsNullOrEmpty(settings.Audience),
                ValidAudience = settings.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2),

                RoleClaimType = ClaimTypes.Role
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, parameters, out validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public static IList<string> GetRolesFromPrincipal(ClaimsPrincipal principal)
        {
            if (principal == null) return new List<string>();

            var roles = new List<string>();

            roles.AddRange(principal.Claims
                .Where(c => c.Type == ClaimTypes.Role || string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value));

            var rolesClaim = principal.Claims.FirstOrDefault(c => string.Equals(c.Type, "roles", StringComparison.OrdinalIgnoreCase));
            if (rolesClaim != null)
            {
                try
                {
                    var arr = JsonConvert.DeserializeObject<string[]>(rolesClaim.Value);
                    if (arr != null) roles.AddRange(arr);
                }
                catch
                {
                    var maybeCsv = rolesClaim.Value;
                    if (!string.IsNullOrWhiteSpace(maybeCsv))
                    {
                        roles.AddRange(maybeCsv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
                    }
                }
            }

            return roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        public static IList<string> GetRolesFromToken(string token, JwtSettings settings)
        {
            if (string.IsNullOrEmpty(token)) return new List<string>();
            var principal = GetPrincipalFromToken(token, settings, out _);
            return GetRolesFromPrincipal(principal!);
        }
    }
}
