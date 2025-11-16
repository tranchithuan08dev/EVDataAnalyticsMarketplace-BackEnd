using System.ComponentModel.DataAnnotations;

namespace EV.DataConsumerService.API.Models.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public Guid? OrganizationId { get; set; }
    }
}
