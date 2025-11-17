namespace EV.AdminService.API.DTOs.DataModels
{
    public class ApiKeyDTO
    {
        public Guid ApiKeyId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool Revoked { get; set; }
    }
}
