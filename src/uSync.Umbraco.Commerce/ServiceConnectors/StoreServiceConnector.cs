using Umbraco.Cms.Core;

namespace uSync.Umbraco.Commerce.ServiceConnectors
{
    /// <summary>
    ///  Define what a store UDI looks like
    /// </summary>
    [UdiDefinition(CommerceConstants.UdiEntityType.Store, UdiType.GuidUdi)]
    public class StoreServiceConnector : CommerceBaseServiceConnector { }

    /// <summary>
    ///  Define what a OrderStatus UDI looks like
    /// </summary>
    [UdiDefinition(CommerceConstants.UdiEntityType.OrderStatus, UdiType.GuidUdi)]
    public class OrderStatusServiceConnector : CommerceBaseServiceConnector { }

    /// <summary>
    ///  Define what a Shipping method UDI looks like
    /// </summary>
    [UdiDefinition(CommerceConstants.UdiEntityType.ShippingMethod, UdiType.GuidUdi)]
    public class ShippingMethodServiceConnector : CommerceBaseServiceConnector { }

    [UdiDefinition(CommerceConstants.UdiEntityType.Country, UdiType.GuidUdi)]
    public class CountryServiceConnector : CommerceBaseServiceConnector { }

    [UdiDefinition(CommerceConstants.UdiEntityType.Currency, UdiType.GuidUdi)]
    public class CurrencyServiceConnector : CommerceBaseServiceConnector { }

    [UdiDefinition(CommerceConstants.UdiEntityType.PaymentMethod, UdiType.GuidUdi)]
    public class PaymentServiceConnector : CommerceBaseServiceConnector { }

    [UdiDefinition(CommerceConstants.UdiEntityType.TaxClass, UdiType.GuidUdi)]
    public class TaxServiceConnector : CommerceBaseServiceConnector { }

    [UdiDefinition(CommerceConstants.UdiEntityType.EmailTemplate, UdiType.GuidUdi)]
    public class EmailTemplateServiceConnector : CommerceBaseServiceConnector { }

    [UdiDefinition(CommerceConstants.UdiEntityType.ExportTemplate, UdiType.GuidUdi)]
    public class ExportTemplateServiceConnector : CommerceBaseServiceConnector { }

    [UdiDefinition(CommerceConstants.UdiEntityType.PrintTemplate, UdiType.GuidUdi)]
    public class PrintTemplateServiceConnector : CommerceBaseServiceConnector { }

}
