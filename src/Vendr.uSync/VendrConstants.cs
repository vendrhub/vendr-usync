namespace Vendr.uSync
{
    internal static class VendrConstants
    {
        public const string Group = "Vendr";

        internal static class Serialization
        {
            public const string Store = "Store";
            public const string Currency = "Currency";
            public const string Country = "Country";
            public const string Region = "Region";

            public const string TaxClass = "TaxClass";

            public const string EmailTemplate = "EmailTemplate";
            public const string PrintTemplate = "PrintTemplate";
            public const string ExportTemplate = "ExportTemplate";
            public const string OrderStatus = "OrderStatus";

            public const string PaymentMethod = "PaymentMethod";
            public const string ShippingMethod = "ShippingMethod";

            public const string ProductAttributes = "ProductAttributes";
        }

        internal static class Priorites
        {
            // content runs at 1200, so by being just below, we will build a store before we sync content
            public const int VENDR_RESERVED_LOWER = 1150;
            public const int VENDR_RESERVED_UPPER = 1199;

            //
            // Handlers are marked with these priorities - setting the order things are imported
            //
            // Some things need to be imported before things they require have been imported (e.g stores)
            // these handlers impliment ISyncPostImportHandler and uSync will run any imports 
            // again at the end of the process (in the post import step). to capture things that might
            // have been missing first time.
            // 
            // Stores, Country & Region are ISyncPostImportHandlers.
            //

            public const int Stores = VENDR_RESERVED_LOWER + 1; // has to be first **

            public const int EmailTemplate = VENDR_RESERVED_LOWER + 2; // requires only store
            public const int OrderStatus = VENDR_RESERVED_LOWER + 3; // requires only store
            public const int PrintTemplate = VENDR_RESERVED_LOWER + 4; // requires only store
            public const int ExportTemplate = VENDR_RESERVED_LOWER + 5; // requires only store

            public const int Country = VENDR_RESERVED_LOWER + 10; // requires store, currency, payment and shipping **
            public const int Region = VENDR_RESERVED_LOWER + 11; // requires store, country, paymeent and shipping **

            public const int Currency = VENDR_RESERVED_LOWER + 12;  // requires countries

            public const int TaxClass = VENDR_RESERVED_LOWER + 13; // requires countries

            public const int PaymentMethod = VENDR_RESERVED_LOWER + 21; // requires store, countries, currencies
            public const int ShippingMethod = VENDR_RESERVED_LOWER + 22;// requires store, countries, currencies

            public const int ProductAttributes = VENDR_RESERVED_LOWER + 23; // requires store
        }

        public static class UdiEntityType
        {
            public const string Store = "vendr-store";

            public const string Country = "vendr-country";

            public const string Region = "vendr-region";

            public const string Currency = "vendr-currency";

            public const string OrderStatus = "vendr-order-status";

            public const string ShippingMethod = "vendr-shipping-method";

            public const string PaymentMethod = "vendr-payment-method";

            public const string TaxClass = "vendr-tax-class";

            public const string EmailTemplate = "vendr-email-template";

            public const string PrintTemplate = "vendr-print-template";

            public const string ExportTemplate = "vendr-export-template";

            public const string Discount = "vendr-discount";

            public const string GiftCard = "vendr-gift-card";

            public const string ProductAttribute = "vendr-product-attribute";
        }
    }
}

