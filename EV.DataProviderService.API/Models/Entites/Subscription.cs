using System;
using System.Collections.Generic;

namespace EV.DataProviderService.API.Models.Entites;

public partial class Subscription
{
    public Guid SubscriptionId { get; set; }

    public Guid ConsumerOrgId { get; set; }

    public Guid DatasetId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public decimal RecurringPrice { get; set; }

    public string Currency { get; set; } = null!;

    public bool Active { get; set; }

    public virtual Organization ConsumerOrg { get; set; } = null!;

    public virtual Dataset Dataset { get; set; } = null!;
}
