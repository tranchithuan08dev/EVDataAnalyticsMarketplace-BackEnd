using EV.AdminService.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EV.AdminService.API.Repositories.Basics
{
    public class CRUDRepository<T> where T : class
    {
        protected readonly EVDataAnalyticsMarketplaceDBContext _context;
        protected readonly DbSet<T> _dbSet;

        public CRUDRepository(EVDataAnalyticsMarketplaceDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Query(bool asNoTracking = true)
        {
            return asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = true, CancellationToken ct = default)
        {
            return await (asNoTracking ? _dbSet.AsNoTracking() : _dbSet).ToListAsync(ct).ConfigureAwait(false);
        }

        public async Task<T?> GetByIdAsync(CancellationToken ct = default, params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues, ct).AsTask().ConfigureAwait(false);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, CancellationToken ct = default)
            => await Query(asNoTracking).Where(predicate).ToListAsync(ct).ConfigureAwait(false);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.AsNoTracking().AnyAsync(predicate, ct).ConfigureAwait(false);

        public async Task CreateAsync(T entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct).ConfigureAwait(false);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            await _context.AddRangeAsync(entities, ct).ConfigureAwait(false);
        }

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        public async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _context.RemoveRange(entities);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        public bool TrySoftDelete(T entity)
        {
            var prop = typeof(T).GetProperty("IsDeleted");
            if (prop != null && prop.PropertyType == typeof(bool))
            {
                prop.SetValue(entity, true);
                _context.Entry(entity).Property("IsDeleted").IsModified = true;
                return true;
            }
            return false;
        }

        public void PrepareCreate(T entity)
        {
            _context.Add(entity);
        }

        public void PrepareUpdate(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
        }

        public void PrepareDelete(T entity)
        {
            _context.Remove(entity);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedListAsync(
            int page,
            int pageSize = 10,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool asNoTracking = true,
            CancellationToken ct = default)
        {
            IQueryable<T> query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
            if (include != null)
            {
                query = include(query);
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalCount = await query.CountAsync(ct).ConfigureAwait(false);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct).ConfigureAwait(false);

            return (items, totalCount);
        }
    }
}
