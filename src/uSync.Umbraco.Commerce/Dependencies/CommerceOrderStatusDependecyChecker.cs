using System.Collections.Generic;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Commerce.Core.Models;
using Umbraco.Extensions;
using uSync.Core.Dependency;

namespace uSync.Umbraco.Commerce.Dependencies
{
    public class CommerceOrderStatusDependecyChecker : ISyncDependencyChecker<OrderStatusReadOnly>
    {
        public UmbracoObjectTypes ObjectType => UmbracoObjectTypes.Unknown;

        public IEnumerable<uSyncDependency> GetDependencies(OrderStatusReadOnly item, DependencyFlags flags)
        {
            return new uSyncDependency
            {
                Name = item.Name,
                Order = CommerceConstants.Priorites.OrderStatus,
                Udi = Udi.Create(CommerceConstants.UdiEntityType.OrderStatus, item.Id)
            }.AsEnumerableOfOne();
        }
    }
}
