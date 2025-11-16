namespace EV.AdminService.API.DTOs.DataModels
{
    public class CreatePaymentRequestDTO
    {
        public Guid? DatasetVersionId { get; set; }
        public Guid? DatasetId { get; set; }
    }
}
