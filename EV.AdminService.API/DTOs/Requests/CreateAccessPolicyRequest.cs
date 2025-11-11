namespace EV.AdminService.API.DTOs.Requests
{
    public class CreateAccessPolicyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AllowedUse { get; set; } = string.Empty;
        public int? ExpiresInDays { get; set; }
    }
}
