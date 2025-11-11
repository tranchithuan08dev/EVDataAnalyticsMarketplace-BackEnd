--PRN232

-- Tạo class Libary + BLL + DAL

-- add reference BLL => DAL, UI => BLL  và DAL

-- Thêm thư viện
--        ****DAL****
<ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.9" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.9">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.9" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.9">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
  --      ****BLL**********
   <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.20" />
  </ItemGroup>

  --  ******** API ******
    <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.20" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.14.0" />
  </ItemGroup>

  --**BẮT BUỘC PHẢI BUILD****

  -- Bước Tiếp theo---
  -- Mở terminal ở DAL

  dotnet ef dbcontext scaffold "Server=Thinh\SQLEXPRESS;Database=SU25DB;User Id=sa;Password=12345;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models

  dotnet ef dbcontext scaffold "Server=Thinh\SQLEXPRESS;Database=FA25BearDB;User Id=sa;Password=12345;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models

  -- Sau đó vào DBContext xóa OnConfiguring
  -- Thêm appsettings.json
    "ConnectionStrings": {
    "DefaultConnection": "Server=Thinh\SQLEXPRESS;Database=SU25DB;User Id=sa;Password=12345;TrustServerCertificate=True"
  },
  -- Vào program.cs thêm dưới dòng builder.Services.AddControllers();

  builder.Services.AddDbContext<....>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
  
  --trong phần DAL folder
     --Interface tạo folder
     --Repository  tạo folder

  -- Đọc đề coi login bằng cái gì 
    --Tạo interface ILoginRepo trong interface DAL
     public interface ILoginRepo
    {
        **Account** GetAccount(string u, string p);
    }
    -- Tạo class LoginRepo trong Repository của DAL

     public class LoginRepo : ILoginRepo
    {
        private readonly TContext _context;

        public LoginRepo(TContext context)
        {
            _context = context;
        }

        public TAccount GetAccount(string u, string p)
        {
          -- Nhớ sửa lại T bằng bảng account trong DB và coi đề bài là gì sửa lại username và password
            return _context.T.FirstOrDefault(
                a => a.T == u && a.T == p
            );
        }
    }
  -- Vào BLL tạo folder Interface và Service
  -- trong Interface tạo interface ILoginService
  public interface ILoginService
    {
        **Account** Login(string u, string p);
    }
 -- trong Service tạo class  LoginService
 public class LoginService : ILoginService
    {

        private readonly ILoginRepo _repository;

        public LoginService(ILoginRepo repository)
        {
            _repository = repository;
        }
        public TAccount Login(string u, string p)
        {
            -- NHớ U và P theo yêu cầu đề bài đề bằng login bằng cái gì ???
            return _repository.GetAccount(u, p);
        }
    }
  -- VÀO PROGRAM.CS ĐĂNG KÝ
  builder.Services.AddScoped<ILoginRepo, LoginRepo>();
  builder.Services.AddScoped<ILoginService, LoginService>();

  -- appsettings.json
  "JwtSettings": {
    "SecretKey": "ksfksdkkdsjlfaskljfiwuirjkdfhajnxnjkashdkf",
    "Issuer": "PEPRN232",
    "Audience": "PEPRN232Clients",
    "ExpiryMinutes": 60 
  },
  -- Tạo interface IJwtService trong folder Interface BLL 
  public interface IJwtService
    {
    //Account đổi bằng bảng account trong DB
        string GenerateToken(**Account** acc);
    }
  -- tạo class JwtService trong folder Service BLL
  public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(**Account** account)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, account.RoleId.ToString()), 
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
    -- VÀO PROGRAM.CS ĐĂNG KÝ 
    var configuration = builder.Configuration;
// 1. Đăng ký IJwtService
builder.Services.AddScoped<IJwtService, JwtService>();

// 2. Cấu hình JWT Bearer Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["JwtSettings:Issuer"],
        ValidAudience = configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
    };
});

// Thêm Authorization Middleware
builder.Services.AddAuthorization();
////////////////////////////////////////////////////////////////////////////////
-- Thêm Middleware sử dụng Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

--Tạo DTOs trong BLL tạo class LoginRequestDTO
 public class LoginRequestDTO
    {
        public string U { get; set; }
        public string P { get; set; }
    }


-- Tạo AuthController
 [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
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
                return BadRequest( new { Error = "HB40001", Message = "Missing/invalid input" });
            }
            if (account.RoleId == 1 || account.RoleId == 2 || account.RoleId == 3)
            {
                return Unauthorized(new {  Error = "HB40101", Message = "Token missing/invalid" });
            }

            var token = _jwtService.GenerateToken(account);

            return Ok(new
            {
                Token = token,
                Role = account.RoleId
            });
        }
    }

