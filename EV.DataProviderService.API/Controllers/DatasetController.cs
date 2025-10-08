//using EV.DataProviderService.API.Models.DTOs;
//using EV.DataProviderService.API.Service;
//using Microsoft.AspNetCore.Mvc;

//namespace EV.DataProviderService.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class DataSourceManagementController : ControllerBase
//    {
//        private readonly IDataSourceService _service;

//        public DataSourceManagementController(IDataSourceService service)
//        {
//            _service = service;
//        }

//        // POST: api/DataSourceManagement/register
//        // Chức năng: Đăng ký nguồn dữ liệu mới
//        [HttpPost("register")]
//        [ProducesResponseType(StatusCodes.Status201Created)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<IActionResult> RegisterDataSource([FromBody] DatasetManagementDataDto registrationDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var newSource = await _service.RegisterNewDataSourceAsync(registrationDto);
//            return CreatedAtAction(nameof(GetDataSource), new { id = newSource.Id }, newSource);
//        }

//        // GET: api/DataSourceManagement/{id}
//        // Chức năng: Lấy thông tin chi tiết của một nguồn dữ liệu
//        [HttpGet("{id}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> GetDataSource(int id)
//        {
//            var source = await _service.GetDataSourceDetailsAsync(id);
//            if (source == null)
//            {
//                return NotFound($"Data Source with ID {id} not found.");
//            }
//            return Ok(source);
//        }

//        // GET: api/DataSourceManagement
//        // Chức năng: Lấy danh sách tất cả nguồn dữ liệu
//        [HttpGet]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        public async Task<IActionResult> GetAllDataSources()
//        {
//            var sources = await _service.ListAllDataSourcesAsync();
//            return Ok(sources);
//        }

//        // PUT: api/DataSourceManagement/{id}
//        // Chức năng: Cập nhật thông tin nguồn dữ liệu
//        [HttpPut("{id}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> UpdateDataSource(int id, [FromBody] DatasetManagementDataDto updateDto, [FromQuery] bool isActive)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var success = await _service.UpdateDataSourceDetailsAsync(id, updateDto, isActive);
//            if (!success)
//            {
//                return NotFound($"Data Source with ID {id} not found.");
//            }
//            return NoContent();
//        }

//        // DELETE: api/DataSourceManagement/{id} (Thực tế là Deactivate)
//        // Chức năng: Vô hiệu hóa nguồn dữ liệu
//        [HttpDelete("{id}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> DeactivateDataSource(int id)
//        {
//            var success = await _service.DeactivateDataSourceAsync(id);
//            if (!success)
//            {
//                return NotFound($"Data Source with ID {id} not found.");
//            }
//            return NoContent();
//        }
//    }
//}