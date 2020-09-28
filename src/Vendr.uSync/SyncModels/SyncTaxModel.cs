using System;

namespace Vendr.uSync.SyncModels
{

    public class SyncTaxModel
    {
        public Guid? CountryId { get; set; }
        public Guid? RegionId { get; set; }
        public decimal Rate { get; set; }
    }
}
