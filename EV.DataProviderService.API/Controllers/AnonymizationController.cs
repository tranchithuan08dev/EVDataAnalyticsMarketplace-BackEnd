using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Service;
using Microsoft.AspNetCore.Mvc;

namespace EV.DataProviderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnonymizationController : ControllerBase
    {
        private readonly IAnonymizationService _service;

        public AnonymizationController(IAnonymizationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Anonymize([FromBody] AnonymizationRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.AnonymizeDatasetAsync(request);
            return Ok(result);
        }
    }
}
