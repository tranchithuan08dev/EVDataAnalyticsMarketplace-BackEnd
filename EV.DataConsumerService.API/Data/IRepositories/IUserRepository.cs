namespace EV.DataConsumerService.API.Data.IRepositories
{
    public interface IUserRepository
    {
        // 1. Lấy OrganizationId từ UserId
        Task<Guid?> GetOrganizationIdByUserIdAsync(Guid userId);

        // 2. Lấy ProviderId từ OrganizationId
        Task<Guid?> GetProviderIdByOrganizationIdAsync(Guid organizationId);
    }
}
