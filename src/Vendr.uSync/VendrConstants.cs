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
            public const int Stores = VENDR_RESERVED_LOWER + 10;

            public const int Currency = VENDR_RESERVED_LOWER + 20;
            public const int Country = VENDR_RESERVED_LOWER + 30;
            public const int Region = VENDR_RESERVED_LOWER + 40;

            public const int TaxClass = VENDR_RESERVED_LOWER + 100;

            public const int EmailTemplate = VENDR_RESERVED_LOWER + 110;
            public const int OrderStatus = VENDR_RESERVED_LOWER + 120;

            public const int PaymentMethod = VENDR_RESERVED_LOWER + 210;
            public const int ShippingMethod = VENDR_RESERVED_LOWER + 220;
        }
    }
}
