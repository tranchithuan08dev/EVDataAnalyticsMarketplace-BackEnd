using System;
using System.Collections.Generic;

namespace EV.DataConsumerService.API.Models.Entities;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid? PurchaseId { get; set; }

    public Guid? SubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public decimal? PaidToProvider { get; set; }

    public decimal? MarketplaceFee { get; set; }

    public DateTime PaidAt { get; set; }

    public string? PaymentGateway { get; set; }

    public string? TransactionReference { get; set; }

    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();
}
