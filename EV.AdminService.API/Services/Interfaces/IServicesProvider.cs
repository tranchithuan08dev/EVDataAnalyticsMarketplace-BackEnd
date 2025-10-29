using EV.AdminService.API.AI.Services.Interfaces;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IServicesProvider
    {
        IUserService UserService { get; }
        IOrganizationService OrganizationService { get; }
        IDataAnalysisService DataAnalysisService { get; }
        IAdminModerationService AdminModerationService { get; }
        IPaymentService PaymentService { get; }
        ISecurityService SecurityService { get; }
        IAnalyticsService AnalyticsService { get; }
    }
}
