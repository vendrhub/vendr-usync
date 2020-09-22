using System;

namespace Vendr.uSync.SyncModels
{
    public class SyncAllowedCountryRegionModel
    {
        public Guid CountryId { get; set; }
        public Guid? RegionId { get; set; }
    }
}
