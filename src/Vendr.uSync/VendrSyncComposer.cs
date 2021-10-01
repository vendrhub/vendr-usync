#if NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Composing;
using Vendr.Core.Models;
using Vendr.Umbraco;
using Vendr.uSync.Serializers;
using uSync8.BackOffice;
using uSync8.Core;
using uSync8.Core.Serialization;
using IComposer = Umbraco.Core.Composing.IUserComposer;
#else
using uSync.BackOffice;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Vendr.Extensions;
using Vendr.Umbraco;
#endif

namespace Vendr.uSync
{
    [ComposeBefore(typeof(uSyncBackOfficeComposer))]
#if NETFRAMEWORK
    [ComposeAfter(typeof(uSyncCoreComposer))]
    [ComposeAfter(typeof(VendrComposer))]
#endif
    public class VendrSyncComposer : IComposer
    {
#if NETFRAMEWORK
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
#else
        public void Compose(IUmbracoBuilder builder)
        {
            // No need to register serializers in v9 as they
            // are auto discovered however we do need to ensure
            // that Vendr has been initialized so we'll call AddVendr
            // which should auto escape if it's already been added
            builder.AddVendr();
        }
#endif
    }
}
