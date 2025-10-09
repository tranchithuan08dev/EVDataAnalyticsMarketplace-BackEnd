namespace EV.AdminService.API.DTOs
{
    public class OrganizationDTO
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; } = null!;
        public string? OrgType { get; set; }
        public string? Country { get; set; }
    }
}
