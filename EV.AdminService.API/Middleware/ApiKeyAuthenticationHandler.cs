using EV.AdminService.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace EV.AdminService.API.Middleware
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder,
            IUnitOfWork unitOfWork) : base(options, logger, encoder)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            var authHeaderText = authHeader.ToString();
            if (!authHeaderText.StartsWith("ApiKey", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Invalid Authorization Header Format. Must start by 'ApiKey '.");
            }

            var rawKey = authHeaderText.Substring("ApiKey ".Length).Trim();
            if (string.IsNullOrEmpty(rawKey))
            {
                return AuthenticateResult.Fail("Null API Key");
            }

            var keyHash = HashApiKey(rawKey);

            var apiKeyRecord = await _unitOfWork.ApiKeyRepository.GetByKeyHashAsync(keyHash);

            if (apiKeyRecord == null || apiKeyRecord.Revoked || (apiKeyRecord.ExpiresAt.HasValue && apiKeyRecord.ExpiresAt.Value < DateTime.UtcNow))
            {
                return AuthenticateResult.Fail("Invalid or Expired API Key");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, apiKeyRecord.ApiKeyId.ToString()),
                new Claim("OrganizationId", apiKeyRecord.OrganizationId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "ApiKeyScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        private byte[] HashApiKey(string apiKey)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(apiKey);
                return sha256.ComputeHash(inputBytes);
            }
        }
    }
}
