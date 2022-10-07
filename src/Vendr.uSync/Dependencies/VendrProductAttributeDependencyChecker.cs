using System.Collections.Generic;
using Vendr.Core.Models;
using uSync.Core.Dependency;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace Vendr.uSync.Dependencies
{
    public class VendrProductAttributeDependencyChecker :
        ISyncDependencyChecker<ProductAttributeReadOnly>
    {
        public UmbracoObjectTypes ObjectType => UmbracoObjectTypes.Unknown;

        public IEnumerable<uSyncDependency> GetDependencies(ProductAttributeReadOnly item, DependencyFlags flags)
            => new uSyncDependency
            {
                Name = item.Name,
                Order = VendrConstants.Priorites.ProductAttributes,
                Udi = Udi.Create(VendrConstants.UdiEntityType.ProductAttribute, item.Id)
            }.AsEnumerableOfOne();
    }
}
