namespace EV.AdminService.API.DTOs.DataModels
{
    public class OrganizationDetailDTO
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; } = null!;
        public string? OrgType { get; set; }
        public string? Country { get; set; }
        public bool IsProvider { get; set; }
        public bool IsConsumer { get; set; }
        public bool IsVerified { get; set; }
    }
}
