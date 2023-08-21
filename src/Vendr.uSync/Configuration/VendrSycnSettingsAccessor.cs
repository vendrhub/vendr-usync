using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Vendr.uSync.Configuration
{
    public class VendrSyncSettingsAccessor
    {
        private readonly IServiceProvider _serviceProvider;

        public VendrSyncSettingsAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public VendrSyncSettings Settings => _serviceProvider.GetRequiredService<IOptions<VendrSyncSettings>>().Value;
    }
}
