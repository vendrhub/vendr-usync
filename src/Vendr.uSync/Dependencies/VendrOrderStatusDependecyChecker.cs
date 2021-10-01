using System.Collections.Generic;
using Vendr.Core.Models;

#if NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Models;
using uSync8.Core.Dependency;
#else
using uSync.Core.Dependency;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;
#endif

namespace Vendr.uSync.Dependencies
{
    public class VendrOrderStatusDependecyChecker : ISyncDependencyChecker<OrderStatusReadOnly>
    {
        public UmbracoObjectTypes ObjectType => UmbracoObjectTypes.Unknown;

        public IEnumerable<uSyncDependency> GetDependencies(OrderStatusReadOnly item, DependencyFlags flags)
        {
            return new uSyncDependency
            {
                Name = item.Name,
                Order = VendrConstants.Priorites.OrderStatus,
                Udi = Udi.Create(VendrConstants.UdiEntityType.OrderStatus, item.Id)
            }.AsEnumerableOfOne();
        }
    }
}
