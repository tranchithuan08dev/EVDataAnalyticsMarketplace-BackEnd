using EV.AdminService.API.Controllers;
using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EV.AdminService.API.Test.Controllers
{
    public class SecurityControllerTests
    {
        [Fact]
        public async Task GetActiveApiKeys_ReturnsOkWithKeys()
        {
            var keys = new List<ApiKeyDTO> { new ApiKeyDTO { ApiKeyId = Guid.NewGuid(), OrganizationName = "Org" } };
            var mockSecurity = new Mock<ISecurityService>();
            mockSecurity.Setup(s => s.GetActiveApiKeyAsync(It.IsAny<CancellationToken>())).ReturnsAsync(keys);

            var mockProvider = new Mock<IServicesProvider>();
            mockProvider.Setup(p => p.SecurityService).Returns(mockSecurity.Object);

            var controller = new SecurityController(mockProvider.Object);
            var result = await controller.GetActiveApiKeys(CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<ApiKeyDTO>>(ok.Value);
            Assert.Single(returned);
        }

        [Fact]
        public async Task RevokeApiKey_ReturnsOk_WhenServiceSucceeds()
        {
            var id = Guid.NewGuid();
            var mockSecurity = new Mock<ISecurityService>();
            mockSecurity.Setup(s => s.RevokeApiKeyAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var mockProvider = new Mock<IServicesProvider>();
            mockProvider.Setup(p => p.SecurityService).Returns(mockSecurity.Object);

            var controller = new SecurityController(mockProvider.Object);
            var result = await controller.RevokeApiKey(id, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RevokeApiKey_ReturnsNotFound_WhenServiceFails()
        {
            var id = Guid.NewGuid();
            var mockSecurity = new Mock<ISecurityService>();
            mockSecurity.Setup(s => s.RevokeApiKeyAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var mockProvider = new Mock<IServicesProvider>();
            mockProvider.Setup(p => p.SecurityService).Returns(mockSecurity.Object);

            var controller = new SecurityController(mockProvider.Object);
            var result = await controller.RevokeApiKey(id, CancellationToken.None);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Notfound", notFound.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }
    }
}
