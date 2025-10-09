namespace EV.AdminService.API.Services.Interfaces
{
    public interface IServicesProvider
    {
        IUserService UserService { get; }
        IModerationService ModerationService { get; }
        IDatasetService DatasetService { get; }
        IPaymentService PaymentService { get; }
        IApiKeyService ApiKeyService { get; }
    }
}
