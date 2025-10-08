using System;
using System.Collections.Generic;

namespace EV.DataProviderService.API.Models.Entites;

public partial class AnonymizationLog
{
    public Guid AnonymizationId { get; set; }

    public Guid DatasetVersionId { get; set; }

    public Guid? PerformedBy { get; set; }

    public string? Method { get; set; }

    public string? Details { get; set; }

    public DateTime PerformedAt { get; set; }

    public virtual DatasetVersion DatasetVersion { get; set; } = null!;
}
