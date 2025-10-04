using System;
using System.Collections.Generic;

namespace EV.DataConsumerService.API.Models.Entities;

public partial class AccessPolicy
{
    public int AccessPolicyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? AllowedUse { get; set; }

    public int? ExpiresInDays { get; set; }

    public DateTime CreatedAt { get; set; }
}
