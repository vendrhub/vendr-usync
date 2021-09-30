
using Umbraco.Core;

namespace Vendr.uSync.ServiceConnectors
{
    /// <summary>
    ///  Define what a store UDI looks like
    /// </summary>
    [UdiDefinition(VendrConstants.UdiEntityType.Store, UdiType.GuidUdi)]
    public class StoreServiceConnector : VendrBaseServiceConnector { }

    /// <summary>
    ///  Define what a OrderStatus UDI looks like
    /// </summary>
    [UdiDefinition(VendrConstants.UdiEntityType.OrderStatus, UdiType.GuidUdi)]
    public class OrderStatusServiceConnector : VendrBaseServiceConnector { }

    /// <summary>
    ///  Define what a Shipping method UDI looks like
    /// </summary>
    [UdiDefinition(VendrConstants.UdiEntityType.ShippingMethod, UdiType.GuidUdi)]
    public class ShippingMethodServiceConnector : VendrBaseServiceConnector { }

    [UdiDefinition(VendrConstants.UdiEntityType.Country, UdiType.GuidUdi)]
    public class CountryServiceConnector : VendrBaseServiceConnector { }

    [UdiDefinition(VendrConstants.UdiEntityType.Currency, UdiType.GuidUdi)]
    public class CurrencyServiceConnector : VendrBaseServiceConnector { }

    [UdiDefinition(VendrConstants.UdiEntityType.PaymentMethod, UdiType.GuidUdi)]
    public class PaymentServiceConnector : VendrBaseServiceConnector { }

    [UdiDefinition(VendrConstants.UdiEntityType.TaxClass, UdiType.GuidUdi)]
    public class TaxServiceConnector : VendrBaseServiceConnector { }

    [UdiDefinition(VendrConstants.UdiEntityType.EmailTemplate, UdiType.GuidUdi)]
    public class EmailTemplateServiceConnector : VendrBaseServiceConnector { }

    [UdiDefinition(VendrConstants.UdiEntityType.ExportTemplate, UdiType.GuidUdi)]
    public class ExportTemplateServiceConnector : VendrBaseServiceConnector { }

    [UdiDefinition(VendrConstants.UdiEntityType.PrintTemplate, UdiType.GuidUdi)]
    public class PrintTemplateServiceConnector : VendrBaseServiceConnector { }

}
