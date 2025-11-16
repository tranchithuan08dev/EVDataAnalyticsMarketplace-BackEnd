using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Models.DTOs;

namespace EV.DataConsumerService.API.Service
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;

        public OrganizationService(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        /// <summary>
        /// Lấy tất cả các tổ chức thông qua Repository.
        /// </summary>
        public async Task<List<OrganizationDto>> GetAllOrganizationsAsync()
        {
            // Logic nghiệp vụ (nếu có, ví dụ: kiểm tra quyền, caching, lọc bổ sung) 
            // sẽ được đặt ở đây. Hiện tại chỉ đơn thuần gọi Repository.
            return await _organizationRepository.GetAllOrganizationsAsync();
        }
    }
}
