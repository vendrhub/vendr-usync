namespace Vendr.uSync
{
    internal static class VendrConstants
    {
        public const string Group = "vendr";

        internal static class Serialization
        {
            public const string Store = "Store";
            public const string Currency = "Currency";
            public const string Country = "Country";
            public const string Region = "Region";

            public const string TaxClass = "TaxClass";

            public const string EmailTemplate = "EmailTemplate";
            public const string OrderStatus = "OrderStatus";

            public const string PaymentMethod = "PaymentMethod";
            public const string ShippingMethod = "ShippingMethod";
        }

        internal static class Priorites
        {
            public const int VENDR_RESERVED_LOWER = 3000;
            public const int VENDR_RESERVED_UPPER = 4000;

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

            public const int Stores = VENDR_RESERVED_LOWER + 10; // has to be first **

            public const int TaxClass = VENDR_RESERVED_LOWER + 20; // requires only on store.
            public const int EmailTemplate = VENDR_RESERVED_LOWER + 30; // requires only store
            public const int OrderStatus = VENDR_RESERVED_LOWER + 40; // requires only store

            public const int Country = VENDR_RESERVED_LOWER + 100; // requires store, currency, payment and shipping **
            public const int Region = VENDR_RESERVED_LOWER + 110; // requires store, country, paymeent and shipping **

            public const int Currency = VENDR_RESERVED_LOWER + 120;  // requires countries

            public const int PaymentMethod = VENDR_RESERVED_LOWER + 210; // requires store, countries, currencies
            public const int ShippingMethod = VENDR_RESERVED_LOWER + 220;// requires store, countries, currencies
        }
    }
}
