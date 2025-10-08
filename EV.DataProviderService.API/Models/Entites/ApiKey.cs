using System;
using System.Collections.Generic;

namespace EV.DataProviderService.API.Models.Entites;

public partial class ApiKey
{
    public Guid ApiKeyId { get; set; }

    public Guid OrganizationId { get; set; }

    public byte[] KeyHash { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool Revoked { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
