//using EV.DataProviderService.API.Data.IRepositories;
//using EV.DataProviderService.API.Models.Entities;
//using Microsoft.EntityFrameworkCore;

//namespace EV.DataProviderService.API.Data.Repositories
//{
//    public class DataSourceRepository : IDataSourceRepository
//    {
//        private readonly EvdataAnalyticsMarketplaceDbContext _context;

//        // DI (Dependency Injection) của DbContext
//        public DataSourceRepository(EvdataAnalyticsMarketplaceDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<DataSource> RegisterDataSourceAsync(DataSource dataSource)
//        {
//            dataSource.RegisteredDate = DateTime.UtcNow; // Ghi lại ngày đăng ký
//            dataSource.IsActive = true; // Mặc định là hoạt động
//            await _context.DataSources.AddAsync(dataSource);
//            await _context.SaveChangesAsync();
//            return dataSource;
//        }

//        public async Task<DataSource> GetDataSourceByIdAsync(int id)
//        {
//            return await _context.DataSources.FindAsync(id);
//        }

//        public async Task<IEnumerable<DataSource>> GetAllDataSourcesAsync()
//        {
//            return await _context.DataSources.ToListAsync();
//        }

//        public async Task<bool> UpdateDataSourceAsync(DataSource updatedSource)
//        {
//            var existingSource = await _context.DataSources.FindAsync(updatedSource.Id);
//            if (existingSource == null) return false;

//            // Cập nhật các trường
//            existingSource.ProviderName = updatedSource.ProviderName;
//            existingSource.DataTypeName = updatedSource.DataTypeName;
//            existingSource.AccessUrl = updatedSource.AccessUrl;
//            existingSource.Description = updatedSource.Description;
//            existingSource.IsActive = updatedSource.IsActive; // Cho phép quản lý trạng thái

//            _context.DataSources.Update(existingSource);
//            await _context.SaveChangesAsync();
//            return true;
//        }

//        public async Task<bool> DeleteDataSourceAsync(int id)
//        {
//            var dataSource = await _context.DataSources.FindAsync(id);
//            if (dataSource == null) return false;

//            // Thường thì nên đặt IsActive = false thay vì xóa hẳn (Soft Delete)
//            _context.DataSources.Remove(dataSource);
//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//}