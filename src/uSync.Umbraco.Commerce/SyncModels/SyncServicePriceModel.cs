using System;

namespace uSync.Umbraco.Commerce.SyncModels
{
    public class SyncServicePriceModel
    {
        public Guid? CurrencyId { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? RegionId { get; set; }

        public decimal Value { get; set; }
    }
}
