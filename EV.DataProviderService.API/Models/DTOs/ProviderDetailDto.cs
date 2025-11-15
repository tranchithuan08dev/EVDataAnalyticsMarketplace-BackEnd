namespace EV.DataProviderService.API.Models.DTOs
{
    public class ProviderDetailDto
    {
        public Guid ProviderId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string? OrgType { get; set; }
        public string? Country { get; set; }
        public string? ContactEmail { get; set; }
        public bool IsVerified { get; set; }
    }
}
