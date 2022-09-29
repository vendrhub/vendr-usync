using Vendr.uSync.Configuration;
using uSync.BackOffice;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Vendr.Extensions;
using Umbraco.Cms.Core;
using Vendr.uSync.ServiceConnectors;
using Microsoft.Extensions.DependencyInjection;

namespace Vendr.uSync
{
    [ComposeBefore(typeof(uSyncBackOfficeComposer))]
    public class VendrSyncComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddOptions<VendrSyncSettings>()
                .Bind(builder.Config.GetSection("Vendr.uSync"));
            builder.Services.AddSingleton<VendrSyncSettingsAccessor>();

            // No need to register serializers in v9 as they
            // are auto discovered however we do need to ensure
            // that Vendr has been initialized so we'll call AddVendr
            // which should auto escape if it's already been added
            builder.AddVendr();

            // in v9 looks like you have to register service connectors.
            // they are autodiscovered in v8.
            UdiParserServiceConnectors.RegisterServiceConnector<StoreServiceConnector>();
            UdiParserServiceConnectors.RegisterServiceConnector<OrderStatusServiceConnector>();
            UdiParserServiceConnectors.RegisterServiceConnector<ShippingMethodServiceConnector>();
            UdiParserServiceConnectors.RegisterServiceConnector<CountryServiceConnector>();
            UdiParserServiceConnectors.RegisterServiceConnector<CurrencyServiceConnector>();
            UdiParserServiceConnectors.RegisterServiceConnector<PaymentServiceConnector>();
            UdiParserServiceConnectors.RegisterServiceConnector<TaxServiceConnector>();
            UdiParserServiceConnectors.RegisterServiceConnector<EmailTemplateServiceConnector>();
            UdiParserServiceConnectors.RegisterServiceConnector<ExportTemplateServiceConnector>();
            UdiParserServiceConnectors.RegisterServiceConnector<PrintTemplateServiceConnector>();
        }
    }
}
