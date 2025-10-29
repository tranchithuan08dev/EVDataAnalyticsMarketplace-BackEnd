using EV.AdminService.API.Controllers;
using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EV.AdminService.API.Test.Controllers
{
    public class AnalyticsControllerTests
    {
        [Fact]
        public async Task GetPopularDatasets_ReturnsOkWithDatasets()
        {
            var mockService = new Mock<IAnalyticsService>();
            var datasets = new List<PopularDatasetDTO> { new PopularDatasetDTO() };
            mockService.Setup(s => s.GetPopularDatasetsAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(datasets);
            var mockProvider = new Mock<IServicesProvider>();
            mockProvider.Setup(p => p.AnalyticsService)
                        .Returns(mockService.Object);
            var controller = new AnalyticsController(mockProvider.Object);
            var result = await controller.GetPopularDatasets(CancellationToken.None);
            
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<PopularDatasetDTO>>(ok.Value);
            Assert.Single(returned);
        }
    }
}
