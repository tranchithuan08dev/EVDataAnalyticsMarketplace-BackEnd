using System;
using System.Collections.Generic;

namespace EV.DataConsumerService.API.Models.Entities;

public partial class DatasetFile
{
    public Guid FileId { get; set; }

    public Guid DatasetVersionId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileUri { get; set; } = null!;

    public long? FileSizeBytes { get; set; }

    public string? Checksum { get; set; }

    public virtual DatasetVersion DatasetVersion { get; set; } = null!;
}
