using EV.AdminService.API.Services.Interfaces;
using System.Security.Claims;

namespace EV.AdminService.API.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeader1 = "X-API-KEY";
        private const string ApiKeyHeader2 = "X-Api-Key";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.Value ?? string.Empty;
            if (!path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var servicesProvider = context.RequestServices.GetService<IServicesProvider>();
            if (servicesProvider == null)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "Server configuration error" });
                return;
            }

            string? key = null;
            if (context.Request.Headers.TryGetValue(ApiKeyHeader1, out var v1))
            {
                key = v1.FirstOrDefault();
            }
            else if (context.Request.Headers.TryGetValue(ApiKeyHeader2, out var v2))
            {
                key = v2.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Missing or empty Api Key" });
                return;
            }

            var ok = await servicesProvider.ApiKeyService.ValidateApiKeyAsync(key, context.RequestAborted).ConfigureAwait(false);
            if (!ok)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid Api Key" });
                return;
            }

            var orgId = await servicesProvider.ApiKeyService.GetOrganizationIdForKeyAsync(key, context.RequestAborted).ConfigureAwait(false);
            if (orgId != Guid.Empty)
            {
                var claimsIdentity = context.User?.Identity as ClaimsIdentity ?? new ClaimsIdentity();
                claimsIdentity.AddClaim(new Claim("org_id", orgId.ToString()));
                context.User = new ClaimsPrincipal(claimsIdentity);
            }

            await _next(context);
        }
    }
}
