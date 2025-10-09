using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using Prometheus;

namespace EV.AdminService.API.Services.Implements
{
    public class ServicesProvider : IServicesProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly Counter _paymentsProcessed;
        private IUserService? _userService;
        private IModerationService? _moderationService;
        private IDatasetService? _datasetService;
        private IPaymentService? _paymentService;
        private IApiKeyService? _apiKeyService;

        public ServicesProvider(IUnitOfWork unitOfWork, HttpClient httpClient, Counter paymentProcessed)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _paymentsProcessed = paymentProcessed;
        }

        public IModerationService ModerationService => _moderationService ??= new ModerationService(_httpClient);
        public IUserService UserService => _userService ??= new UserService(_unitOfWork);
        public IDatasetService DatasetService => _datasetService ??= new DatasetService(_unitOfWork);
        public IPaymentService PaymentService => _paymentService ??= new PaymentService(_unitOfWork, _paymentsProcessed);
        public IApiKeyService ApiKeyService => _apiKeyService ??= new ApiKeyService(_unitOfWork);
    }
}
