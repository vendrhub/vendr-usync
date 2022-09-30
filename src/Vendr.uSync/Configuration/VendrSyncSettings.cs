using System;

namespace Vendr.uSync.Configuration
{
    public class VendrSyncSettings
    {
        public VendrSyncPaymentMethodSettings PaymentMethods { get; set; }

        public VendrSyncSettings()
        {
            PaymentMethods = new VendrSyncPaymentMethodSettings();
        }
    }

    public class VendrSyncPaymentMethodSettings
    {
        public string[] IgnoreSettings { get; set; }

        public VendrSyncPaymentMethodSettings()
        {
            IgnoreSettings = Array.Empty<string>();
        }
    }
}
