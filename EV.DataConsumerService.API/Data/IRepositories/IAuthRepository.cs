namespace EV.DataConsumerService.API.Data.IRepositories
{
    public interface IAuthRepository
    {
        Task<Guid> RegisterUserAsync(string email, byte[] passwordHash, string displayName, Guid? organizationId, int roleId);
        Task<bool> UserExistsAsync(string email);
        Task<(Guid UserId, byte[] PasswordHash, int RoleId)?> GetUserCredentialsAsync(string email);
        Task<string> GetRoleNameByIdAsync(int roleId);
    }
}
