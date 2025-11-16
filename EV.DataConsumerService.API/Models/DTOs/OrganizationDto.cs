namespace EV.DataConsumerService.API.Models.DTOs
{
    public class OrganizationDto
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string OrgType { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
