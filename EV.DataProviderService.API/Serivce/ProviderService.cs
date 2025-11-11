using EV.DataProviderService.API.Data.IRepositories;
using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;

namespace EV.DataProviderService.API.Service;

public class ProviderService : IProviderService
{
    private readonly IProviderRepository _repository;

    public ProviderService(IProviderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProviderListDto>> GetAllProvidersAsync()
    {
        var providers = await _repository.GetAllProvidersAsync();
        return providers.Select(MapToProviderListDto);
    }

    public async Task<ProviderListDto?> GetProviderByIdAsync(Guid providerId)
    {
        var provider = await _repository.GetProviderWithDetailsAsync(providerId);
        if (provider == null) return null;
        return MapToProviderListDto(provider);
    }

    private ProviderListDto MapToProviderListDto(Provider provider)
    {
        var datasets = provider.Datasets?.ToList() ?? new List<Dataset>();
        var allVersions = datasets
            .SelectMany(d => d.DatasetVersions ?? new List<DatasetVersion>())
            .ToList();

        // Extract data types from datasets
        var dataTypes = datasets
            .Where(d => !string.IsNullOrEmpty(d.DataTypes))
            .SelectMany(d => d.DataTypes!.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(dt => dt.Trim())
            .Distinct()
            .ToList();

        // Calculate pricing summary
        var pricingSummary = CalculatePricingSummary(allVersions);

        // Calculate sharing policies summary
        var sharingPolicies = CalculateSharingPoliciesSummary(datasets);

        return new ProviderListDto
        {
            ProviderId = provider.ProviderId,
            OrganizationName = provider.Organization?.Name ?? string.Empty,
            OrgType = provider.Organization?.OrgType,
            OrganizationDescription = provider.Organization?.Description,
            Country = provider.Organization?.Country,
            ContactEmail = provider.ContactEmail,
            Verified = provider.Verified,
            OnboardedAt = provider.OnboardedAt,
            TotalDatasets = datasets.Count,
            ActiveDatasets = datasets.Count(d => !string.IsNullOrEmpty(d.Status) && d.Status.ToLower() == "approved"),
            PendingDatasets = datasets.Count(d => !string.IsNullOrEmpty(d.Status) && d.Status.ToLower() == "pending"),
            AvailableDataTypes = dataTypes,
            PricingSummary = pricingSummary,
            SharingPolicies = sharingPolicies
        };
    }

    private PricingSummaryDto? CalculatePricingSummary(List<DatasetVersion> versions)
    {
        if (versions == null || !versions.Any())
            return null;

        var versionsWithPricing = versions
            .Where(v => v.PricePerDownload.HasValue || v.PricePerGb.HasValue)
            .ToList();

        if (!versionsWithPricing.Any())
            return null;

        var pricesPerDownload = versionsWithPricing
            .Where(v => v.PricePerDownload.HasValue)
            .Select(v => v.PricePerDownload!.Value)
            .ToList();

        var pricesPerGb = versionsWithPricing
            .Where(v => v.PricePerGb.HasValue)
            .Select(v => v.PricePerGb!.Value)
            .ToList();

        return new PricingSummaryDto
        {
            MinPricePerDownload = pricesPerDownload.Any() ? pricesPerDownload.Min() : null,
            MaxPricePerDownload = pricesPerDownload.Any() ? pricesPerDownload.Max() : null,
            MinPricePerGb = pricesPerGb.Any() ? pricesPerGb.Min() : null,
            MaxPricePerGb = pricesPerGb.Any() ? pricesPerGb.Max() : null,
            HasSubscriptionPricing = versions.Any(v => v.SubscriptionRequired),
            DatasetsWithPricing = versionsWithPricing
                .Select(v => v.DatasetId)
                .Distinct()
                .Count()
        };
    }

    private SharingPoliciesSummaryDto? CalculateSharingPoliciesSummary(List<Dataset> datasets)
    {
        if (datasets == null || !datasets.Any())
            return null;

        var licenseTypes = datasets
            .Where(d => !string.IsNullOrEmpty(d.LicenseType))
            .Select(d => d.LicenseType!)
            .Distinct()
            .ToList();

        var researchOnlyCount = datasets.Count(d => 
            d.LicenseType?.ToLower().Contains("research") == true ||
            d.LicenseType?.ToLower() == "research-only");

        var commercialCount = datasets.Count(d => 
            d.LicenseType?.ToLower().Contains("commercial") == true ||
            d.LicenseType?.ToLower() == "commercial");

        var extendedCount = datasets.Count(d => 
            d.LicenseType?.ToLower().Contains("extended") == true ||
            d.LicenseType?.ToLower() == "extended");

        return new SharingPoliciesSummaryDto
        {
            ResearchOnlyCount = researchOnlyCount,
            CommercialCount = commercialCount,
            ExtendedRightsCount = extendedCount,
            LicenseTypes = licenseTypes
        };
    }
}

