using BLL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using DAL.Models;
using BLL.DTOs;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net.NetworkInformation;

namespace PRN231PE_SP24B3_PracticalExam_SE184492.Controllers
{
   
        [Route("api/[controller]")]
        [ApiController]
        public class SongController : ControllerBase
        {
            private readonly IRepoService _repoService;

            public SongController(IRepoService repoService)
            {
                _repoService = repoService;
            }

            [HttpGet]

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
                    SongID = t.SongId,
                    Title = t.Title,
                    Duration = t.Duration,
                    AlbumID = t.AlbumId,
                    TitleAlbum = t.Album.Title,
                    ArtistAlbum = t.Album.Artist,
                    ReleaseAlbum = t.Album.ReleaseDate
                });

                return Ok(result); // 200
            }

            [HttpGet("{id}")]
            
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
                        SongID = acc.SongId,
                        Title = acc.Title,
                        Duration = acc.Duration,
                        AlbumID = acc.AlbumId,
                        TitleAlbum = acc.Album.Title,
                        ArtistAlbum = acc.Album.Artist,
                        ReleaseAlbum = acc.Album.ReleaseDate
                    };

                    return Ok(result); // 200 OK
                }
                catch (UnauthorizedAccessException)
                {
                    // Token sai hoặc không đủ quyền
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
                        Message = "Lỗi hệ thống.",
                        Details = ex.Message
                    });
                }
            }
            [HttpDelete("{id}")]
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
                        Message = $"Đã xóa thành công."
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
        public async Task<IActionResult> Create([FromBody] SongDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Error = "HB40001", Message = "Dữ liệu không hợp lệ hoặc trống." });

            // Ánh xạ từ DTO sang Entity
            var profile = new Song
            {
                Title = dto.Title,
                Duration = dto.Duration,
                AlbumId = dto.AlbumId
            };

            var created = await _repoService.AddAsync(profile);

            return StatusCode(201, new
            {
                message = "Thành công."
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SongDTO dto)
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
                existing.Title = dto.Title;
                existing.Duration = dto.Duration;
                existing.AlbumId = dto.AlbumId;

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
}
    

