using System;

#if NETFRAMEWORK
using System.Configuration;
using System.Linq;
#endif


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
#if NETFRAMEWORK
            IgnoreSettings = (ConfigurationManager.AppSettings["Vendr.uSync:PaymentMethods:IgnoreSettings"] ?? "")
                .Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
#else
            IgnoreSettings = Array.Empty<string>();
#endif
        }
    }
}
