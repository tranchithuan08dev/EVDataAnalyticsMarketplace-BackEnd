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
        UserRepository UserRepository { get; }
        OrganizationRepository OrganizationRepository { get; }
        RoleRepository RoleRepository { get; }
        ProviderRepository ProviderRepository { get; }
        DatasetVersionRepository DatasetVersionRepository { get; }
        DataQualityFlagRepository DataQualityFlagRepository { get; }
        DatasetRepository DatasetRepository { get; }
        PaymentRepository PaymentRepository { get; }
        ApiKeyRepository ApiKeyRepository { get; }
        PurchaseRepository PurchaseRepository { get; }
        SubscriptionRepository SubscriptionRepository { get; }
        AnalysisRepository AnalysisRepository { get; }
        AccessPolicyRepository AccessPolicyRepository { get; }
    }
}
