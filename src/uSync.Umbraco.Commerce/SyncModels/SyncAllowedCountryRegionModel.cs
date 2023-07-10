using System;

namespace uSync.Umbraco.Commerce.SyncModels
{
    public class SyncAllowedCountryRegionModel
    {
        public Guid CountryId { get; set; }
        public Guid? RegionId { get; set; }
    }
}
