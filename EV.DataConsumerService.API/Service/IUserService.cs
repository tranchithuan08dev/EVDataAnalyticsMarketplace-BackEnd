namespace EV.DataConsumerService.API.Service
{
    public interface IUserService
    {
        Task<Guid?> GetProviderIdByUserIdAsync(Guid userId);
    }
}
