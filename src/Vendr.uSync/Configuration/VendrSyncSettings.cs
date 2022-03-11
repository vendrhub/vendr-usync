using System;

#if NETFRAMEWORK
using System.Configuration;
using System.Linq;
#endif


namespace Vendr.uSync.Configuration
{
    public class VendrSyncSettings
    {
        public VendrSyncPaymentProviderSettings PaymentProviders { get; set; }

        public VendrSyncSettings()
        {
            PaymentProviders = new VendrSyncPaymentProviderSettings();
        }
    }

    public class VendrSyncPaymentProviderSettings
    {
        public string[] IgnoreSettings { get; set; }

        public VendrSyncPaymentProviderSettings()
        {
#if NETFRAMEWORK
            IgnoreSettings = (ConfigurationManager.AppSettings["Vendr.uSync:PaymentProviders:IgnoreSettings"] ?? "")
                .Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
#else
            IgnoreSettings = Array.Empty<string>();
#endif
        }
    }
}
