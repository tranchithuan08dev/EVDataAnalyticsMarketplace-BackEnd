namespace EV.AdminService.API.DTOs.DataModels
{
    public class UserDetailDTO
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? DisplayName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? OrganizationId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
