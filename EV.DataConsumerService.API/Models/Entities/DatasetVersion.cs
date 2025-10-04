using System;
using System.Collections.Generic;

namespace EV.DataConsumerService.API.Models.Entities;

public partial class DatasetVersion
{
    public Guid DatasetVersionId { get; set; }

    public Guid DatasetId { get; set; }

    public string VersionLabel { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string FileFormat { get; set; } = null!;

    public long? FilesizeBytes { get; set; }

    public string? StorageUri { get; set; }

    public bool IsAnalyzed { get; set; }

    public string? AnalysisReportUri { get; set; }

    public string? SampleUri { get; set; }

    public decimal? PricePerDownload { get; set; }

    public decimal? PricePerGb { get; set; }

    public bool SubscriptionRequired { get; set; }

    public int? AccessPolicyId { get; set; }

    public string? LicenseText { get; set; }

    public virtual ICollection<Analysis> Analyses { get; set; } = new List<Analysis>();

    public virtual ICollection<AnonymizationLog> AnonymizationLogs { get; set; } = new List<AnonymizationLog>();

    public virtual Dataset Dataset { get; set; } = null!;

    public virtual ICollection<DatasetFile> DatasetFiles { get; set; } = new List<DatasetFile>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
