using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV.AdminService.API.Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EVDataAnalyticsMarketplaceDBContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        private UserRepository? _userRepository;
        private OrganizationRepository? _organizationRepository;
        private RoleRepository? _roleRepository;
        private ProviderRepository? _providerRepository;
        private DatasetVersionRepository? _datasetVersionRepository;
        private DataQualityFlagRepository? _dataQualityFlagRepository;
        private DatasetRepository? _datasetRepository;
        private PaymentRepository? _paymentRepository;
        private ApiKeyRepository? _apiKeyRepository;
        private PurchaseRepository? _purchaseRepository;
        private SubscriptionRepository? _subscriptionRepository;
        private AnalysisRepository? _analysisRepository;
        private AccessPolicyRepository? _accessPolicyRepository;

        public UnitOfWork(EVDataAnalyticsMarketplaceDBContext context)
        {
            _context = context;
        }

        public UserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public OrganizationRepository OrganizationRepository => _organizationRepository ??= new OrganizationRepository(_context);
        public RoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);
        public ProviderRepository ProviderRepository => _providerRepository ??= new ProviderRepository(_context);
        public DatasetVersionRepository DatasetVersionRepository => _datasetVersionRepository ??= new DatasetVersionRepository(_context);
        public DataQualityFlagRepository DataQualityFlagRepository => _dataQualityFlagRepository ??= new DataQualityFlagRepository(_context);
        public DatasetRepository DatasetRepository => _datasetRepository ??= new DatasetRepository(_context);
        public PaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);
        public ApiKeyRepository ApiKeyRepository => _apiKeyRepository ??= new ApiKeyRepository(_context);
        public PurchaseRepository PurchaseRepository => _purchaseRepository ??= new PurchaseRepository(_context);
        public SubscriptionRepository SubscriptionRepository => _subscriptionRepository ??= new SubscriptionRepository(_context);
        public AnalysisRepository AnalysisRepository => _analysisRepository ??= new AnalysisRepository(_context);
        public AccessPolicyRepository AccessPolicyRepository => _accessPolicyRepository ??= new AccessPolicyRepository(_context);

        void IDisposable.Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            await _context.DisposeAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null) return _transaction;
            _transaction = await _context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
            return _transaction;
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction == null) return;
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            await _transaction.CommitAsync(ct).ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync(ct).ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
        {
            await using (var transaction = await _context.Database.BeginTransactionAsync(ct).ConfigureAwait(false))
            {
                try
                {
                    await action(ct);
                    await _context.SaveChangesAsync(ct).ConfigureAwait(false);
                    await transaction.CommitAsync(ct).ConfigureAwait(false);
                }
                catch
                {
                    await transaction.RollbackAsync(ct).ConfigureAwait(false);
                    throw;
                }
            }
        }
    }
}
