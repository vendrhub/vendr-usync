using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace uSync.Umbraco.Commerce.Configuration
{
    public class CommerceSyncSettingsAccessor
    {
        private readonly IServiceProvider _serviceProvider;

        public CommerceSyncSettingsAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CommerceSyncSettings Settings => _serviceProvider.GetRequiredService<IOptions<CommerceSyncSettings>>().Value;
    }
}
