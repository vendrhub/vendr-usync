#if NET
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
#endif

namespace Vendr.uSync.Configuration
{
    public class VendrSyncSettingsAccessor
    {
#if NETFRAMEWORK
        public VendrSyncSettings Settings { get; }

        public VendrSyncSettingsAccessor(VendrSyncSettings settings)
        {
            Settings = settings;
        }
#else
        private readonly IServiceProvider _serviceProvider;

        public VendrSyncSettingsAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public VendrSyncSettings Settings => _serviceProvider.GetRequiredService<IOptions<VendrSyncSettings>>().Value;
#endif
    }
}
