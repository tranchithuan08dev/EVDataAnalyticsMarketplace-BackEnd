using EV.AdminService.API.Helpers;
using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV.AdminService.API.Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EVDataAnalyticsMarketplaceContext _context;
        private readonly JwtHelper _jwtHelper;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        private UserRepository? _userRepository;
        private ProviderRepository? _providerRepository;
        private ConsumerRepository? _consumerRepository;
        private RoleRepository? _roleRepository;
        private DatasetRepository? _datasetRepository;
        private PurchaseRepository? _purchaseRepository;
        private PaymentRepository? _paymentRepository;
        private RevenueShareRepository? _revenueShareRepository;
        private ApiKeyRepository? _apiKeyRepository;

        public UnitOfWork(EVDataAnalyticsMarketplaceContext context, JwtHelper helper)
        {
            _context = context;
            _jwtHelper = helper;
        }

        public UserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public ProviderRepository ProviderRepository => _providerRepository ??= new ProviderRepository(_context);
        public ConsumerRepository ConsumerRepository => _consumerRepository ??= new ConsumerRepository(_context);
        public RoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);
        public JwtHelper JwtHelper => _jwtHelper;
        public DatasetRepository DatasetRepository => _datasetRepository ??= new DatasetRepository(_context);
        public PurchaseRepository PurchaseRepository => _purchaseRepository ??= new PurchaseRepository(_context);
        public PaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);
        public RevenueShareRepository RevenueShareRepository => _revenueShareRepository ??= new RevenueShareRepository(_context);
        public ApiKeyRepository ApiKeyRepository => _apiKeyRepository ??= new ApiKeyRepository(_context);

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
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null) return _transaction;
            _transaction = await _context.Database.BeginTransactionAsync(ct);
            return _transaction;
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction == null) return;
            await _context.SaveChangesAsync(ct);
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
        {
            await using (var transaction = await _context.Database.BeginTransactionAsync(ct))
            {
                try
                {
                    await action(ct);
                    await _context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            }
        }
    }
}
