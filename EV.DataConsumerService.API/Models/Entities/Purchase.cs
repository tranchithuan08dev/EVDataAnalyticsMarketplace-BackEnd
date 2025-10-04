using System;
using System.Collections.Generic;

namespace EV.DataConsumerService.API.Models.Entities;

public partial class Purchase
{
    public Guid PurchaseId { get; set; }

    public Guid ConsumerOrgId { get; set; }

    public Guid? BuyerUserId { get; set; }

    public Guid DatasetVersionId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string Currency { get; set; } = null!;

    public DateTime PurchasedAt { get; set; }

    public DateTime? AccessExpiresAt { get; set; }

    public string? TransactionId { get; set; }

    public virtual Organization ConsumerOrg { get; set; } = null!;

    public virtual DatasetVersion DatasetVersion { get; set; } = null!;
}
