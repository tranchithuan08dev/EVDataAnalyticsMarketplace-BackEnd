namespace EV.AdminService.API.DTOs.Requests
{
    public class ImportDatasetRequest
    {
        public IFormFile? MetadataFile { get; set; }
        public IFormFile? DataFile { get; set; }
    }
}
