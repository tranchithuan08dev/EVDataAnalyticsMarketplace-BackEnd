using Amazon.S3;
using EV.AdminService.API.AI.Services.Implements;
using EV.AdminService.API.AI.Services.Interfaces;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.ML;

namespace EV.AdminService.API.Services.Implements
{
    public class ServicesProvider : IServicesProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<DataConsumerService> _logger;
        private readonly MLContext _mlContext;
        private IUserService? _userService;
        private IOrganizationService? _organizationService;
        private IDataAnalysisService? _dataAnalysisService;
        private IAdminModerationService? _adminModerationService;
        private IPaymentService? _paymentService;
        private ISecurityService? _securityService;
        private IAnalyticsService? _analyticsService;
        private IRoleService? _roleService;
        private IPolicyService? _policyService;
        private ISubscriptionService? _subscriptionService;
        private IProviderImportService? _providerImportService;
        private IDataConsumerService? _dataConsumerService;

        public ServicesProvider(
            IUnitOfWork unitOfWork, 
            MLContext mlContext, 
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory, 
            IAmazonS3 s3Client,
            ILogger<DataConsumerService> logger)
        {
            _unitOfWork = unitOfWork;
            _mlContext = mlContext;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _s3Client = s3Client;
            _logger = logger;
        }
        public IUserService UserService => _userService ??= new UserService(_unitOfWork);
        public IOrganizationService OrganizationService => _organizationService ??= new OrganizationService(_unitOfWork);
        public IDataAnalysisService DataAnalysisService => _dataAnalysisService ??= new DataAnalysisService(_mlContext);
        public IAdminModerationService AdminModerationService => _adminModerationService ??= new AdminModerationService(_unitOfWork);
        public IPaymentService PaymentService => _paymentService ??= new PaymentService(_unitOfWork);
        public ISecurityService SecurityService => _securityService ??= new SecurityService(_unitOfWork);
        public IAnalyticsService AnalyticsService => _analyticsService ??= new AnalyticsService(_unitOfWork, _mlContext, _configuration, _httpClientFactory);
        public IRoleService RoleService => _roleService ??= new RoleService(_unitOfWork);
        public IPolicyService PolicyService => _policyService ??= new PolicyService(_unitOfWork);
        public ISubscriptionService SubscriptionService => _subscriptionService ??= new SubscriptionService(_unitOfWork);
        public IProviderImportService ProviderImportService => _providerImportService ??= new ProviderImportService(_unitOfWork, _s3Client, _configuration);
        public IDataConsumerService DataConsumerService => _dataConsumerService ??= new DataConsumerService(_unitOfWork, _s3Client, _configuration, _logger);
    }
}
