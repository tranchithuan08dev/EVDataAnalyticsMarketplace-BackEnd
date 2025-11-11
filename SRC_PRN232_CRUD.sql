-- tạo IRepo trong interface DAL
  public interface IRepo
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<T> AddAsync(T entity);

        Task<T> UpdateByIdAsync(int id, T entity);

        Task<bool> DeleteByIdAsync(int id);
    }
-- tạo Repo trong Repository DAL

  public class Repo : IRepo
    {
        private readonly TContext _context;

        public Repo(TContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tbảng>> GetAllAsync()
        {
            return await _context.Tbảng
                                 .Include(l => l.Tbảngkếtnoi)
                                 .ToListAsync();
        }

        public async Task<Tbảng> GetByIdAsync(int id)
        {
            return await _context.Tbảng
                                 .Include(l => l.Tbảngkếtnoi)
                                 .FirstOrDefaultAsync(l => l.TbảngId == id);
        }

        public async Task<Tbảng> AddAsync(Tbảng entity)
        {
            await _context.Tbảng.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Tbảng> UpdateByIdAsync(int id, Tbảng entity)
        {
            var existing = await _context.Tbảng.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var existing = await _context.Tbảng.FindAsync(id);
            if (existing == null) return false;

            _context.Tbảng.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
-- Tạo IRepoService trong Interface BLL
public interface IRepoService
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<T> AddAsync(T entity);

        Task<T> UpdateByIdAsync(int id, T entity);

        Task<bool> DeleteByIdAsync(int id);
    }
-- Tạo RepoService trong Service BLL
   public class RepoService : IRepoService
    {


        private readonly IRepo _repository;

        public RepoService(IRepo repository)
        {
            _repository = repository;
        }
        public Task<Tbảng> AddAsync(Tbảng entity)
        {
            return _repository.AddAsync(entity);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            return _repository.DeleteByIdAsync(id);
           
        }

        public Task<IEnumerable<Tbảng>> GetAllAsync()
        {
        
            return _repository.GetAllAsync();
        }

        public Task<Tbảng> GetByIdAsync(int id)
        {
            
            return _repository.GetByIdAsync(id);
        }

        public Task<Tbảng> UpdateByIdAsync(int id, Tbảng entity)
        {
            return _repository.UpdateByIdAsync(id, entity);
        }
    }

    -- Đăng kí
    builder.Services.AddScoped<IRepo, Repo>();
    builder.Services.AddScoped<IRepoService, RepoService>();

    -- Tạo Controller
      [Route("api/[controller]")]
    [ApiController]
    public class TBangController : ControllerBase
    {
        private readonly IRepoService _repoService;

        public TBangController(IRepoService repoService)
        {
            _repoService = repoService;
        }

         [HttpGet]
        [Authorize(Roles = "4,5,6,7")] cấu hình theo đề bài 
        public async Task<IActionResult> GetAll()
        {
            var list = await _repoService.GetAllAsync();

            if (list == null || !list.Any())
            {
                return StatusCode(400, new
                {
                    error = "HB40001",
                    message = "Dữ liệu không hợp lệ."
                });
            }

            var result = list.Select(t => new
            {
              ProfileId =  t.ProfileId,
               ... cấu hình theo đề bài yêu cầu map ra thường thì sẽ 2 bảng
            });

            return Ok(result); // 200
        }
       
        [HttpGet("{id}")]
        [Authorize(Roles = "4,5,6,7")] cấu hình lại 
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var acc = await _repoService.GetByIdAsync(id);

                if (acc == null)
                    return StatusCode(400, new
                    {
                        error = "HB40001",
                        message = "Dữ liệu không hợp lệ."
                    });

                
                var result = new
                {
                    acc.ProfileId,
                     ... cấu hình theo đề bài yêu cầu map ra thường thì sẽ 2 bảng
                };

                return Ok(result); // 200 OK
            }
            catch (UnauthorizedAccessException)
            {
                // Token sai hoặc không đủ quyền
                return StatusCode(401, new
                {
                    Error = "HB40101",
                    Message = "Token missing/invalid hoặc không đủ quyền truy cập."sửa lại
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "HB50001",
                    Message = "Lỗi hệ thống.",sửa
                    Details = ex.Message
                });
            }
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "5,6")] cấu hình lại 
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        Error = "HB40001",
                        Message = "ID không hợp lệ."
                    });
                }

                // Gọi service để xóa
                var deleted = await _repoService.DeleteByIdAsync(id);

                if (!deleted)
                {
                    return StatusCode(400, new
                    {
                        error = "HB40001",
                        message = "Dữ liệu không hợp lệ."
                    });

                }

                // Trả về 200 nếu xóa thành công
                return Ok(new
                {
                    Message = $"Đã xóa thành công." sửa lại 
                });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, new
                {
                    Error = "HB40101",
                    Message = "Token missing/invalid hoặc không đủ quyền truy cập."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "HB50001",
                    Message = "Lỗi hệ thống khi xóa dữ liệu.",
                    Details = ex.Message
                });
            }
        }

         [HttpPost]
        [Authorize(Roles = "5,6")] sửa
        public async Task<IActionResult> Create([FromBody] tạoDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Error = "HB40001", Message = "Dữ liệu không hợp lệ hoặc trống." });

            // Ánh xạ từ DTO sang Entity
            var profile = new Profile đổ theo bảng cần tạo
            {
              TypeId = dto.TypeId,
              mapping những cái từ Dto tới entity
               
            };

            var created = await _repoService.AddAsync(profile);

            return StatusCode(201, new
            {
                message = "Thành công."
            });
        }
         [HttpPut("{id}")]
        [Authorize(Roles = "4,5,6,7")] sửa
        public async Task<IActionResult> Update(int id, [FromBody] tạoDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        Error = "HB40001",
                        Message = "Dữ liệu gửi lên không hợp lệ hoặc trống."
                    });
                }

                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        Error = "HB40002",
                        Message = "ID không hợp lệ."
                    });
                }

                // Kiểm tra có tồn tại không
                var existing = await _repoService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new
                    {
                        Error = "HB40401",
                        Message = $"Không tìm thấy ."
                    });
                }

                // Ánh xạ DTO sang entity
                existing.TypeId = dto.TypeId;
                maping theo ddeef

                // Gọi BLL để cập nhật
                var updated = await _repoService.UpdateByIdAsync(id, existing);

                return Ok(new
                {
                    Message = $"Cập nhật thành công.",
                
                });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, new
                {
                    Error = "HB40101",
                    Message = "Token thiếu hoặc không hợp lệ."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "HB50001",
                    Message = "Lỗi hệ thống khi cập nhật hồ sơ báo.",
                    Details = ex.Message
                });
            }
        }
     
    }

    --Test SWAGGER program.cs

    builder.Services.AddSwaggerGen(c =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Chỉ dán mỗi token, không cần thêm 'Bearer'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});