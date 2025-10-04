using System;
using System.Collections.Generic;

namespace EV.DataConsumerService.API.Models.Entities;

public partial class AccessLog
{
    public long AccessLogId { get; set; }

    public Guid? OrganizationId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? DatasetVersionId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime ActionAt { get; set; }

    public string? IpAddress { get; set; }

    public string? Details { get; set; }
}
