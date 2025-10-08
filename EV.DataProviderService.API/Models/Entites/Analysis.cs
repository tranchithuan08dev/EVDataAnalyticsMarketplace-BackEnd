using System;
using System.Collections.Generic;

namespace EV.DataProviderService.API.Models.Entites;

public partial class Analysis
{
    public Guid AnalysisId { get; set; }

    public Guid? DatasetVersionId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? ReportUri { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Visibility { get; set; }

    public virtual DatasetVersion? DatasetVersion { get; set; }
}
