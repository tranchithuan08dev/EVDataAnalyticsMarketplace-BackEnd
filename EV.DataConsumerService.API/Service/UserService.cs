using EV.DataConsumerService.API.Data.IRepositories;

namespace EV.DataConsumerService.API.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Guid?> GetProviderIdByUserIdAsync(Guid userId)
        {
            // 1. Lấy OrganizationId
            Guid? orgId = await _userRepository.GetOrganizationIdByUserIdAsync(userId);

            if (!orgId.HasValue)
            {
                // Người dùng không tồn tại hoặc không có OrganizationId
                return null;
            }

            // 2. Lấy ProviderId dựa trên OrganizationId
            // Kiểm tra xem Organization này có phải là Provider hay không
            Guid? providerId = await _userRepository.GetProviderIdByOrganizationIdAsync(orgId.Value);

            return providerId;
        }
    }
}
