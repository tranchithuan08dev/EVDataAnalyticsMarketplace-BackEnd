using System;
using System.Collections.Generic;

namespace EV.DataProviderService.API.Models.Entites;

public partial class Dataset
{
    public Guid DatasetId { get; set; }

    public Guid ProviderId { get; set; }

    public string Title { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? LongDescription { get; set; }

    public string? Category { get; set; }

    public string? DataTypes { get; set; }

    public string? Region { get; set; }

    public string? VehicleTypes { get; set; }

    public string? BatteryTypes { get; set; }

    public string? LicenseType { get; set; }

    public string Visibility { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<DatasetVersion> DatasetVersions { get; set; } = new List<DatasetVersion>();

    public virtual Provider Provider { get; set; } = null!;

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
