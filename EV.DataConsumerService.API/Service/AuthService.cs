using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EV.DataConsumerService.API.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        // Giả định: 2: provider, 3: consumer
        private const int CONSUMER_ROLE_ID = 3;
        private const int PROVIDER_ROLE_ID = 2;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        // --- Helper: Hash password (SHA512 - 64 bytes)
        private byte[] HashPassword(string password)
        {
            using (var sha = SHA512.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password)); // output 64 bytes
            }
        }

        // --- Helper: Verify password (so sánh 64 bytes)
        private bool VerifyPassword(string password, byte[] storedHash)
        {
            using (var sha = SHA512.Create())
            {
                var computedHash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

                return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
            }
        }

        // --- Helper: Tạo JWT Token ---
        private string GenerateJwtToken(Guid userId, string roleName)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        // --- Logic Đăng ký ---
        public async Task<(bool Success, string Message)> Register(UserRegisterDto userRegisterDto)
        {
            if (await _authRepository.UserExistsAsync(userRegisterDto.Email))
            {
                return (false, "Email đã tồn tại.");
            }

            int roleId = userRegisterDto.OrganizationId.HasValue
                ? CONSUMER_ROLE_ID
                : PROVIDER_ROLE_ID;

            // Hash password 64 bytes
            byte[] passwordHash = HashPassword(userRegisterDto.Password);

            try
            {
                await _authRepository.RegisterUserAsync(
                    userRegisterDto.Email,
                    passwordHash,
                    userRegisterDto.DisplayName,
                    userRegisterDto.OrganizationId,
                    roleId
                );

                return (true, "Đăng ký thành công.");
            }
            catch (Exception)
            {
                return (false, "Lỗi server khi đăng ký người dùng.");
            }
        }

        // --- Logic Đăng nhập ---
        public async Task<AuthResponseDto> Login(UserLoginDto userLoginDto)
        {
            var userCredentials = await _authRepository.GetUserCredentialsAsync(userLoginDto.Email);

            if (userCredentials == null)
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

            byte[] storedHash = userCredentials.Value.PasswordHash;

            // Kiểm tra đúng chuẩn 64 bytes
            if (storedHash == null || storedHash.Length != 64)
                throw new InvalidOperationException("PasswordHash trong DB không hợp lệ (phải là 64 bytes).");

            // Verify password
            if (!VerifyPassword(userLoginDto.Password, storedHash))
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

            // Lấy role
            string roleName = await _authRepository.GetRoleNameByIdAsync(userCredentials.Value.RoleId);

            if (string.IsNullOrEmpty(roleName))
                throw new InvalidOperationException("Không tìm thấy Role của người dùng.");

            // Tạo JWT
            string token = GenerateJwtToken(userCredentials.Value.UserId, roleName);

            return new AuthResponseDto
            {
                Token = token,
                Email = userLoginDto.Email,
                Role = roleName
            };
        }
    }
}
