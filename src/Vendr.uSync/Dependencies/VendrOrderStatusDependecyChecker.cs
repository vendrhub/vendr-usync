using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Umbraco.Core;
using Umbraco.Core.Models;

using uSync8.Core.Dependency;

using Vendr.Core.Models;

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
