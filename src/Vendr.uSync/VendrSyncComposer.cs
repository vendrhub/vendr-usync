
using Umbraco.Core;
using Umbraco.Core.Composing;

using uSync8.BackOffice;
using uSync8.Core;
using uSync8.Core.Serialization;

using Vendr.Core.Models;
using Vendr.uSync.Serializers;

namespace Vendr.uSync
{
    [ComposeAfter(typeof(uSyncCoreComposer))]
    [ComposeBefore(typeof(uSyncBackOfficeComposer))]
    public class VendrSyncComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ISyncSerializer<StoreReadOnly>, StoreSerializer>();
            composition.Register<ISyncSerializer<CurrencyReadOnly>, CurrencySerializer>();
            composition.Register<ISyncSerializer<CountryReadOnly>, CountrySerializer>();
        }
    }
}
