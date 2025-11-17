using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _unitOfWork.UserRepository.GetByIdAsync(ct, userId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserDetailDTO>> GetUsersAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.UserRepository.GetUsersAsync(ct).ConfigureAwait(false);
        }

        public async Task<bool> SetUserStatusAsync(Guid userId, bool isActive, CancellationToken ct = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(ct, userId).ConfigureAwait(false);
            if (user == null)
            {
                return false;
            }

            user.IsActive = isActive;
            await _unitOfWork.UserRepository.UpdateAsync(user, ct).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> UpdateUserRolesAsync(Guid userId, List<int> roleIds, CancellationToken ct = default)
        {
            var user = await _unitOfWork.UserRepository.GetUserWithRoleAsync(userId, ct).ConfigureAwait(false);
            if (user == null)
            {
                return false;
            }

            user.Roles.Clear();

            var newRoles = await _unitOfWork.RoleRepository.GetRolesAsync(roleIds, ct).ConfigureAwait(false);

            await _unitOfWork.UserRepository.AddRolesAsync(user, newRoles, ct).ConfigureAwait(false);

            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> VerifyProviderAsync(Guid organizationId, CancellationToken ct = default)
        {
            var provider = await _unitOfWork.ProviderRepository.GetByOrganizationIdAsync(organizationId, ct).ConfigureAwait(false);

            if (provider == null)
            {
                return false;
            }

            provider.Verified = true;
            await _unitOfWork.ProviderRepository.UpdateAsync(provider, ct).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }
    }
}
