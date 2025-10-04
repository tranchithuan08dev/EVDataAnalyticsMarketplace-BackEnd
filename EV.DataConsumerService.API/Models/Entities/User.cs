using System;
using System.Collections.Generic;

namespace EV.DataConsumerService.API.Models.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;

    public byte[]? PasswordHash { get; set; }

    public string? DisplayName { get; set; }

    public Guid? OrganizationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
