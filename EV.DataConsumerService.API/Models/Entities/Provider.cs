using System;
using System.Collections.Generic;

namespace EV.DataConsumerService.API.Models.Entities;

public partial class Provider
{
    public Guid ProviderId { get; set; }

    public Guid OrganizationId { get; set; }

    public string? ContactEmail { get; set; }

    public bool Verified { get; set; }

    public DateTime? OnboardedAt { get; set; }

    public virtual ICollection<Dataset> Datasets { get; set; } = new List<Dataset>();

    public virtual Organization Organization { get; set; } = null!;
}
