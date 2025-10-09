using EV.DataConsumerService.API.Models.DTOs;
using EV.DataConsumerService.API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace EV.DataConsumerService.API.Controllers
{
    // Route sẽ là /odata/Datasets, khớp với EntitySet bạn đã đăng ký
    [Route("[controller]")]
    // Kế thừa ODataController để tận dụng các tính năng OData
    public class DatasetsController : ODataController
    {
        private readonly IDatasetService _datasetService;

        // Dependency Injection: Nhận IDatasetService để truy cập tầng nghiệp vụ
        public DatasetsController(IDatasetService datasetService)
        {
            _datasetService = datasetService;
        }

        [HttpGet]
        [EnableQuery] // Thuộc tính quan trọng nhất cho OData
        public IQueryable<DatasetSearchResultDto> Get()
        {
            // Controller chỉ ủy quyền cho Service và trả về IQueryable<T>. 
            // Logic lọc (như Status='approved', Visibility='public') và ánh xạ đều nằm ở tầng Service.
            return _datasetService.GetPublicDatasetsForSearch();
        }
    }
    }
