namespace EV.AdminService.API.DTOs.DataModels
{
    public class ProcessTransactionResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
