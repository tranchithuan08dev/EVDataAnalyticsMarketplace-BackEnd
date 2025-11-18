using EV.DataConsumerService.API.Service;
using Microsoft.AspNetCore.Mvc;

namespace EV.DataConsumerService.API.Controllers
{
   
        [Route("api/[controller]")]
        [ApiController]
        public class UsersController : ControllerBase
        {
            private readonly IUserService _userService;

            public UsersController(IUserService userService)
            {
                _userService = userService;
            }

            /// <summary>
            /// Lấy ProviderId liên kết với một UserId.
            /// Thường dùng cho người dùng đã đăng nhập (lấy UserId từ JWT Claim).
            /// </summary>
            /// <param name="userId">ID của người dùng.</param>
            [HttpGet("{userId}/provider-id")]
            // [Authorize] // Đảm bảo chỉ người dùng có quyền mới được gọi
            [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid?))]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> GetProviderIdByUserId(Guid userId)
            {
                if (userId == Guid.Empty)
                {
                    return BadRequest("UserId không hợp lệ.");
                }

                try
                {
                    Guid? providerId = await _userService.GetProviderIdByUserIdAsync(userId);

                    if (!providerId.HasValue)
                    {
                        // Trả về 404 nếu người dùng tồn tại nhưng không phải là Provider
                        // hoặc người dùng không tồn tại.
                        return NotFound(new { Message = $"Không tìm thấy ProviderId cho UserId: {userId}." });
                    }

                    return Ok(providerId.Value);
                }
                catch (Exception ex)
                {
                    // Nên log ex ở đây
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Lỗi server trong quá trình xử lý yêu cầu.", Details = ex.Message });
                }
            }
        }
    }

