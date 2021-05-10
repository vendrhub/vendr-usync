
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

            composition.Register<ISyncSerializer<EmailTemplateReadOnly>, EmailTemplateSerializer>();
            composition.Register<ISyncSerializer<PrintTemplateReadOnly>, PrintTemplateSerializer>();
            composition.Register<ISyncSerializer<ExportTemplateReadOnly>, ExportTemplateSerializer>();
            composition.Register<ISyncSerializer<OrderStatusReadOnly>, OrderStatusSerializer>();
            composition.Register<ISyncSerializer<PaymentMethodReadOnly>, PaymentMethodSeralizer>();

            composition.Register<ISyncSerializer<RegionReadOnly>, RegionSerializer>();
            composition.Register<ISyncSerializer<ShippingMethodReadOnly>, ShippingMethodSerializer>();
            composition.Register<ISyncSerializer<TaxClassReadOnly>, TaxClassSerializer>();

        }
    }
}
