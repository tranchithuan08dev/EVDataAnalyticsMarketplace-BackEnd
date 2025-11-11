using BLL.DTOs;
using BLL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PRN231PE_SP24B3_PracticalExam_SE184492.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _service;
        private readonly IJwtService _jwtService;
        public AuthController(ILoginService service, IJwtService jwtService)
        {
            _service = service;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTO login)
        {
            var account = _service.Login(login.U, login.P);

            if (account == null)
            {
                return BadRequest(new { Error = "HB40001", Message = "Missing/invalid input" });
            }
            if (account.UserRole1 == 1 || account.UserRole1 == 3)
            {
                return Unauthorized(new { Error = "HB40101", Message = "Token missing/invalid" });
            }

            var token = _jwtService.GenerateToken(account);

            return Ok(new
            {
                Token = token,
                Role = account.UserRole1
            });
        }
    }
}
