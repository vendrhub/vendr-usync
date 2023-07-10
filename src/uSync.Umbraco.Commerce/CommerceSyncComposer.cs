using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Commerce.Extensions;
using uSync.BackOffice;
using uSync.Umbraco.Commerce.Configuration;
using uSync.Umbraco.Commerce.ServiceConnectors;

namespace uSync.Umbraco.Commerce
{
    [ComposeBefore(typeof(uSyncBackOfficeComposer))]
    public class CommerceSyncComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddOptions<CommerceSyncSettings>()
                .Bind(builder.Config.GetSection("Commerce.uSync"));
            builder.Services.AddSingleton<CommerceSyncSettingsAccessor>();

            // No need to register serializers in v9 as they
            // are auto discovered however we do need to ensure
            // that Commerce has been initialized so we'll call AddCommerce
            // which should auto escape if it's already been added
            builder.AddUmbracoCommerce();

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
