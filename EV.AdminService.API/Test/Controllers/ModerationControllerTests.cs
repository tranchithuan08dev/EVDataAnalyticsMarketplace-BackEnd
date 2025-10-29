using EV.AdminService.API.Controllers;
using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;

namespace EV.AdminService.API.Test.Controllers
{
    public class ModerationControllerTests
    {
        [Fact]
        public async Task GetPendingDatasets_ReturnsOkWithDatasets()
        {
            var items = new List<PendingModerationDTO> { new PendingModerationDTO() };
            var mockService = new Mock<IAdminModerationService>();
            mockService.Setup(s => s.GetPendingDatasetsAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(items);

            var mockProvider = new Mock<IServicesProvider>();
            mockProvider.Setup(p => p.AdminModerationService)
                        .Returns(mockService.Object);

            var controller = new ModerationController(mockProvider.Object);
            var result = await controller.GetPendingDatasets(CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<PendingModerationDTO>>(ok.Value);
            Assert.Single(returned);
        }

        [Fact]
        public async Task GetModerationDetail_ReturnsOk_WhenServicesSucceeds()
        {
            var id = Guid.NewGuid();
            var dto = new ModerationDetailDTO();
            var mockService = new Mock<IAdminModerationService>();
            mockService.Setup(s => s.GetModerationDetailAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            var mockProvider = new Mock<IServicesProvider>();
            mockProvider.Setup(p => p.AdminModerationService)
                .Returns(mockService.Object);

            var controller = new ModerationController(mockProvider.Object);
            var result = await controller.GetModerationDetail(id, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(dto, ok.Value);
        }

        [Fact]
        public async Task GetModerationDetail_ReturnsNotFound_WhenServiceFails()
        {
            var id = Guid.NewGuid();
            var dto = new ModerationDetailDTO();
            var mockService = new Mock<IAdminModerationService>();
            mockService.Setup(s => s.GetModerationDetailAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            var mockProvider = new Mock<IServicesProvider>();
            mockProvider.Setup(p => p.AdminModerationService)
                .Returns(mockService.Object);

            var controller = new ModerationController(mockProvider.Object);
            var result = await controller.GetModerationDetail(id, CancellationToken.None);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Notfound", notFound.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ApproveDataset_BehavesAccordingToService(bool serviceResult)
        {
            var id = Guid.NewGuid();
            var mockService = new Mock<IAdminModerationService>();
            mockService.Setup(s => s.ApproveDatasetAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

            var mockProvider = new Mock<IServicesProvider>();
            mockProvider.Setup(p => p.AdminModerationService).Returns(mockService.Object);

            var controller = new ModerationController(mockProvider.Object);
            var result = await controller.ApproveDataset(id, CancellationToken.None);

            if (serviceResult)
            {
                var ok = Assert.IsType<OkObjectResult>(result);
                Assert.Contains("approved", ok.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                var badRequest = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Contains("not in pending", badRequest.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RejectDataset_BehavesAccordingToService(bool serviceResult)
        {
            var id = Guid.NewGuid();
            var mockService = new Mock<IAdminModerationService>();
            mockService.Setup(s => s.RejectDatasetAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(serviceResult);

            var mockProvider = new Mock<IServicesProvider>();
            mockProvider.Setup(p => p.AdminModerationService).Returns(mockService.Object);

            var controller = new ModerationController(mockProvider.Object);
            var result = await controller.ApproveDataset(id, CancellationToken.None);

            if (serviceResult)
            {
                var ok = Assert.IsType<OkObjectResult>(result);
                Assert.Contains("rejected", ok.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                var badRequest = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Contains("not in pending", badRequest.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
