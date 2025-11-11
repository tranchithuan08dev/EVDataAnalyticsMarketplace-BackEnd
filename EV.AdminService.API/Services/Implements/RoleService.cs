using EV.AdminService.API.DTOs.Requests;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Role> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct = default)
        {
            var newRole = new Role
            {
                Name = request.Name,
                Description = request.Description
            };
            await _unitOfWork.RoleRepository.CreateAsync(newRole, ct).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            return newRole;
        }

        public async Task<bool> DeleteRoleAsync(int roleId, CancellationToken ct = default)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(ct, roleId).ConfigureAwait(false);
            if (role == null) return false;

            var userHasRole = await _unitOfWork.UserRepository.AnyAsync(u => u.Roles.Any(r => r.RoleId == roleId), ct);
            if (userHasRole)
            {
                return false;
            }

            await _unitOfWork.RoleRepository.DeleteAsync(role, ct).ConfigureAwait(false);
            return true;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.RoleRepository.GetAllAsync(true, ct).ConfigureAwait(false);
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId, CancellationToken ct = default)
        {
            return await _unitOfWork.RoleRepository.GetByIdAsync(ct, roleId).ConfigureAwait(false);
        }

        public async Task<bool> UpdateRoleAsync(int roleId, UpdateRoleRequest request, CancellationToken ct = default)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(ct, roleId).ConfigureAwait(false);
            if (role == null) return false;

            role.Name = request.Name;
            role.Description = request.Description;
            await _unitOfWork.RoleRepository.UpdateAsync(role, ct).ConfigureAwait(false);
            return true;
        }
    }
}
