using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    //[Authorize(Policy = "AdminOnly")]
    public class PaymentController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        private readonly IConfiguration _configuration;
        public PaymentController(IServicesProvider servicesProvider, IConfiguration configuration)
        {
            _servicesProvider = servicesProvider;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePaymentRequest([FromBody] CreatePaymentRequestDTO request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Token không hợp lệ hoặc không chứa OrganizationId.");
            }

            var user = await _servicesProvider.UserService.GetUserByIdAsync(userId, ct).ConfigureAwait(false);

            if (user == null || user.OrganizationId == null)
            {
                return Unauthorized("Không tìm thấy thông tin tổ chức (Organization) liên kết với tài khoản của bạn.");
            }

            var organization = await _servicesProvider.OrganizationService.GetOrganizationByIdAsync(user.OrganizationId.Value, ct).ConfigureAwait(false);

            if (organization == null || organization.Consumer == null)
            {
                return Unauthorized("Không tìm thấy tổ chức (Organization) Consumer của bạn.");
            }

            try
            {
                var result = await _servicesProvider.PaymentService.CreatePaymentRequestAsync(request, organization.OrganizationId, ct);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ khi tạo thanh toán." });
            }
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> SepayWebhook([FromBody] SepayWebhookPayload payload, CancellationToken ct)
        {
            try
            {
                var expectedApiKey = _configuration["SepayPersonalSettings:WebhookSecret"];

                if (string.IsNullOrEmpty(expectedApiKey))
                {
                    return StatusCode(500, new { message = "Lỗi cấu hình hệ thống." });
                }

                if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    return Unauthorized(new { message = "Thiếu token xác thực." });
                }

                var providedAuthValue = authHeader.ToString();
                var expectedPrefix = "Apikey ";

                if (!providedAuthValue.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return Unauthorized(new { message = "Token không đúng định dạng." });
                }

                var providedKey = providedAuthValue.Substring(expectedPrefix.Length).Trim();

                if (providedKey != expectedApiKey)
                {
                    return Unauthorized(new { message = "API Key không hợp lệ." });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi xác thực." });
            }

            var result = await _servicesProvider.PaymentService.ProcessSepayWebhookAsync(payload, ct);

            if (result.IsSuccessful)
            {
                return Ok(new { message = result.Message });
            }

            return BadRequest(new { message = result.Message });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPayments(CancellationToken ct = default)
        {
            var payments = await _servicesProvider.PaymentService.GetPendingPaymentsAsync(ct);
            return Ok(payments);
        }

        [HttpPost("distribute/{paymentId}")]
        public async Task<IActionResult> DistributePayment([FromRoute] Guid paymentId, CancellationToken ct = default)
        {
            var result = await _servicesProvider.PaymentService.DistributeRevenueAsync(paymentId, ct);
            if (!result)
            {
                return BadRequest("Cannot distribute revenue. Recheck PaymentId/Provider.OrganizationId or payment already distributed.");
            }

            return Ok();
        }
    }
}
