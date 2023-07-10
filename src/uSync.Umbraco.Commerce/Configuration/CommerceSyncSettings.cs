using System;

namespace uSync.Umbraco.Commerce.Configuration
{
    public class CommerceSyncSettings
    {
        public CommerceSyncPaymentMethodSettings PaymentMethods { get; set; }

        public CommerceSyncSettings()
        {
            PaymentMethods = new CommerceSyncPaymentMethodSettings();
        }
    }

    public class CommerceSyncPaymentMethodSettings
    {
        public string[] IgnoreSettings { get; set; }

        public CommerceSyncPaymentMethodSettings()
        {
            IgnoreSettings = Array.Empty<string>();
        }
    }
}
