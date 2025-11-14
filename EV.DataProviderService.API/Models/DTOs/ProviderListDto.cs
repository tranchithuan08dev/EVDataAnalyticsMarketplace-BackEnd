namespace EV.DataProviderService.API.Models.DTOs;

public class ProviderListDto
{
    public Guid ProviderId { get; set; }
    
    // Organization Information
    public string OrganizationName { get; set; } = null!;
    public Guid OrganizationId { get; set; }
    public string? OrgType { get; set; } // hãng xe, trạm sạc, fleet operators, ...
    public string? OrganizationDescription { get; set; }
    public string? Country { get; set; }
    
    // Provider Information
    public string? ContactEmail { get; set; }
    public bool Verified { get; set; }
    public DateTime? OnboardedAt { get; set; }
    
    // Data Source Summary
    public int TotalDatasets { get; set; }
    public int ActiveDatasets { get; set; } // Datasets with status = "approved"
    public int PendingDatasets { get; set; } // Datasets with status = "pending"
    
    // Data Types Available
    public List<string> AvailableDataTypes { get; set; } = new(); // pin, hành trình, sạc, giao dịch điện
    
    // Pricing Information Summary
    public PricingSummaryDto? PricingSummary { get; set; }
    
    // Sharing Policies Summary
    public SharingPoliciesSummaryDto? SharingPolicies { get; set; }

    // Dataset Versions
    public Guid DatasetVersionId { get; set; }
}

public class PricingSummaryDto
{
    public decimal? MinPricePerDownload { get; set; }
    public decimal? MaxPricePerDownload { get; set; }
    public decimal? MinPricePerGb { get; set; }
    public decimal? MaxPricePerGb { get; set; }
    public bool HasSubscriptionPricing { get; set; }
    public int DatasetsWithPricing { get; set; }
}

public class SharingPoliciesSummaryDto
{
    public int ResearchOnlyCount { get; set; }
    public int CommercialCount { get; set; }
    public int ExtendedRightsCount { get; set; }
    public List<string> LicenseTypes { get; set; } = new();
}
