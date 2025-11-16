using EV.DataConsumerService.API.Models.DTOs;
using EV.DataConsumerService.API.Service;
using Microsoft.AspNetCore.Mvc;

namespace EV.DataConsumerService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OrganizationDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOrganizations()
        {
            try
            {
                var organizations = await _organizationService.GetAllOrganizationsAsync();
                return Ok(organizations);
            }
            catch (Exception ex)
            {
                // Nên log ex ở đây
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Lỗi server trong quá trình lấy danh sách tổ chức.", Details = ex.Message });
            }
        }
    }
}
