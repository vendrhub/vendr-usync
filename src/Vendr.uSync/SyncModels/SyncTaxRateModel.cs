using System;

namespace Vendr.uSync.SyncModels
{
    public class SyncTaxRateModel
    {
        public Guid? CountryId { get; set; }
        public Guid? RegionId { get; set; }
        public decimal Rate { get; set; }
    }
}
