//using EV.DataProviderService.API.Data.IRepositories;
//using EV.DataProviderService.API.Models.DTOs;
//using EV.DataProviderService.API.Models.Entities;

//namespace EV.DataProviderService.API.Service
//{
//    public class DataSourceService : IDataSourceService
//    {
//        private readonly IDataSourceRepository _repository;

//        public DataSourceService(IDataSourceRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task<DataSource> RegisterNewDataSourceAsync(DataSourceRegistrationDto registrationDto)
//        {
//            // Logic nghiệp vụ: Ánh xạ DTO sang Entity
//            var newDataSource = new DataSource
//            {
//                ProviderName = registrationDto.ProviderName,
//                DataTypeName = registrationDto.DataTypeName,
//                AccessUrl = registrationDto.AccessUrl,
//                Description = registrationDto.Description
//                // RegisteredDate và IsActive sẽ được set trong Repository
//            };

//            // Có thể thêm logic kiểm tra trùng lặp tại đây trước khi lưu
//            // if (await _repository.IsDuplicate(newDataSource.AccessUrl)) throw new Exception("Data source already exists.");

//            return await _repository.RegisterDataSourceAsync(newDataSource);
//        }

//        public async Task<DataSource> GetDataSourceDetailsAsync(int id)
//        {
//            return await _repository.GetDataSourceByIdAsync(id);
//        }

//        public async Task<IEnumerable<DataSource>> ListAllDataSourcesAsync()
//        {
//            return await _repository.GetAllDataSourcesAsync();
//        }

//        public async Task<bool> UpdateDataSourceDetailsAsync(int id, DataSourceRegistrationDto updateDto, bool isActive)
//        {
//            var existingSource = await _repository.GetDataSourceByIdAsync(id);
//            if (existingSource == null) return false;

//            // Cập nhật các trường từ DTO
//            existingSource.ProviderName = updateDto.ProviderName;
//            existingSource.DataTypeName = updateDto.DataTypeName;
//            existingSource.AccessUrl = updateDto.AccessUrl;
//            existingSource.Description = updateDto.Description;
//            existingSource.IsActive = isActive; // Cập nhật trạng thái

//            return await _repository.UpdateDataSourceAsync(existingSource);
//        }

//        public async Task<bool> DeactivateDataSourceAsync(int id)
//        {
//            var existingSource = await _repository.GetDataSourceByIdAsync(id);
//            if (existingSource == null) return false;

//            existingSource.IsActive = false; // Vô hiệu hóa nguồn dữ liệu
//            return await _repository.UpdateDataSourceAsync(existingSource);
//        }
//    }
//}