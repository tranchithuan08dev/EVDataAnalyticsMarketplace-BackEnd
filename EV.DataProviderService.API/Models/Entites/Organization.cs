using System;
using System.Collections.Generic;

namespace EV.DataProviderService.API.Models.Entites;

public partial class Organization
{
    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string? OrgType { get; set; }

    public string? Description { get; set; }

    public string? Country { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();

    public virtual Consumer? Consumer { get; set; }

    public virtual Provider? Provider { get; set; }

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
