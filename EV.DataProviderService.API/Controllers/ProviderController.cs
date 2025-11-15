using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Service;
using Microsoft.AspNetCore.Mvc;

namespace EV.DataProviderService.API.Controllers;

/// <summary>
/// Controller quản lý Nhà cung cấp dữ liệu (Data Providers: hãng xe, trạm sạc, fleet operators, ...)
/// </summary>
[Route("DataProvider")]
[ApiController]
public class ProviderController : ControllerBase
{
    private readonly IProviderService _providerService;
    private readonly ILogger<ProviderController> _logger;

    public ProviderController(IProviderService providerService, ILogger<ProviderController> logger)
    {
        _providerService = providerService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả Nhà cung cấp dữ liệu
    /// </summary>
    /// <returns>Danh sách providers với thông tin đăng ký, quản lý nguồn dữ liệu, chính sách chia sẻ và giá</returns>
    /// <response code="200">Trả về danh sách providers thành công</response>
    /// <response code="404">Không tìm thấy provider nào</response>
    /// <response code="500">Lỗi server</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProviderListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllProviders()
    {
        try
        {
            var providers = await _providerService.GetAllProvidersAsync();
            
            if (providers == null || !providers.Any())
            {
                _logger.LogWarning("No providers found in the system");
                return NotFound(new { message = "Không tìm thấy nhà cung cấp dữ liệu nào." });
            }

            _logger.LogInformation("Retrieved {Count} providers successfully", providers.Count());
            return Ok(providers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving providers");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách nhà cung cấp dữ liệu.", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một Nhà cung cấp dữ liệu theo ID
    /// </summary>
    /// <param name="providerId">ID của provider</param>
    /// <returns>Thông tin chi tiết của provider</returns>
    /// <response code="200">Trả về thông tin provider thành công</response>
    /// <response code="404">Không tìm thấy provider</response>
    /// <response code="500">Lỗi server</response>
    [HttpGet("{providerId}")]
    [ProducesResponseType(typeof(ProviderListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProviderById(Guid providerId)
    {
        try
        {
            var provider = await _providerService.GetProviderByIdAsync(providerId);
            
            if (provider == null)
            {
                _logger.LogWarning("Provider with ID {ProviderId} not found", providerId);
                return NotFound(new { message = $"Không tìm thấy nhà cung cấp dữ liệu với ID: {providerId}" });
            }

            _logger.LogInformation("Retrieved provider {ProviderId} successfully", providerId);
            return Ok(provider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving provider {ProviderId}", providerId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy thông tin nhà cung cấp dữ liệu.", error = ex.Message });
        }
    }
}

