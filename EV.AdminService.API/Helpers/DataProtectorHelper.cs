using Microsoft.AspNetCore.DataProtection;

namespace EV.AdminService.API.Helpers
{
    public class DataProtectorHelper
    {
        private readonly IDataProtector _protector;
        public DataProtectorHelper(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("EVDataAnalyticsMarketplace.ProtectSensitive");
        }

        public string Protect(string plaintText)
        {
            if (string.IsNullOrEmpty(plaintText))
            {
                return plaintText ?? string.Empty;
            }
            return _protector.Protect(plaintText);
        }

        public string Unprotect(string protectedText)
        {
            if (string.IsNullOrEmpty(protectedText))
            {
                return protectedText ?? string.Empty;
            }

            try
            {
                return _protector.Unprotect(protectedText);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
