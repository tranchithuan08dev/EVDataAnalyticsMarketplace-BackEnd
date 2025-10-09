using EV.AdminService.API.Helpers;
using EV.AdminService.API.Repositories.Implements;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV.AdminService.API.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        JwtHelper JwtHelper { get; }
        UserRepository UserRepository { get; }
        ProviderRepository ProviderRepository { get; }
        ConsumerRepository ConsumerRepository { get; }
        RoleRepository RoleRepository { get; }
        DatasetRepository DatasetRepository { get; }
        PaymentRepository PaymentRepository { get; }
        RevenueShareRepository RevenueShareRepository { get; }
        PurchaseRepository PurchaseRepository { get; }
        ApiKeyRepository ApiKeyRepository { get; }
    }
}
