using System;
using System.Collections.Generic;

namespace EV.DataProviderService.API.Models.Entites;

public partial class Consumer
{
    public Guid ConsumerId { get; set; }

    public Guid OrganizationId { get; set; }

    public string? ContactEmail { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
