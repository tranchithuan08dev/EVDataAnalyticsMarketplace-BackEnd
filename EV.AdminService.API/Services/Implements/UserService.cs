using EV.AdminService.API.Models.DTOs;
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

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.UserRepository.GetUserDtosAsync(ct);
        }

        public async Task<object?> GetConsumersAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.ConsumerRepository.GetConsumer(ct);
        }

        public async Task<object?> GetProvidersAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.ProviderRepository.GetProvider(ct);
        }

        public async Task<object?> GetUsersByRoleAsync(string role, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(role)) return null;
            role = role.Trim();

            if (role.Equals("Provider", StringComparison.OrdinalIgnoreCase))
            {
                var providers = await _unitOfWork.ProviderRepository.GetProvider(ct);
                return providers;
            }

            if (role.Equals("Consumer", StringComparison.OrdinalIgnoreCase))
            {
                var consumers = await _unitOfWork.ConsumerRepository.GetConsumer(ct);
                return consumers;
            }

            var roleEntity = await _unitOfWork.RoleRepository.GetByNameAsync(role, ct);
            if (roleEntity == null) return null;

            var users = await _unitOfWork.UserRepository.GetUserDTOsByRoleId(roleEntity, ct);

            return users;
        }

        public async Task<bool> SetUserActiveAsync(Guid userId, bool isActive, CancellationToken ct = default)
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

        public async Task<bool> UpdateUserRolesAsync(Guid userId, IEnumerable<string> roles, CancellationToken ct = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(ct, userId);
            if (user == null)
            {
                return false;
            }

            var roleNames = roles?.Where(r => !string.IsNullOrWhiteSpace(r))
                                .Select(r => r.Trim())
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToList() ?? new List<string>();

            var roleEntities = await _unitOfWork.RoleRepository.GetByNamesAsync(roleNames, ct);

            user.Roles.Clear();
            foreach (var role in roleEntities)
            {
                user.Roles.Add(role);
            }

            await _unitOfWork.UserRepository.UpdateAsync(user, ct).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }
    }
}
