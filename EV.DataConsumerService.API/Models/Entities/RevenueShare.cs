using System;
using System.Collections.Generic;

namespace EV.DataConsumerService.API.Models.Entities;

public partial class RevenueShare
{
    public Guid RevenueShareId { get; set; }

    public Guid PaymentId { get; set; }

    public Guid ToOrganizationId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Payment Payment { get; set; } = null!;

    public virtual Organization ToOrganization { get; set; } = null!;
}
