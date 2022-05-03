using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Vendr.Common;
using Vendr.Core.Api;
using Vendr.Core.Models;

using Vendr.uSync.Extensions;
using Vendr.uSync.SyncModels;
using Vendr.uSync.Configuration;

#if NETFRAMEWORK
using Umbraco.Core.Logging;
using uSync8.Core.Extensions;
#else
using Microsoft.Extensions.Logging;
using uSync.Core;
#endif

namespace Vendr.uSync.Serializers
{
    /// <summary>
    ///  Base serializer for method (payment/shipping) serailizers that share some common value types. 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public abstract class MethodSerializerBase<TObject> : VendrSerializerBase<TObject>
        where TObject : EntityBase
    {
        protected MethodSerializerBase(IVendrApi vendrApi, VendrSyncSettingsAccessor settingsAccessor,
            IUnitOfWorkProvider uowProvider,
#if NETFRAMEWORK
            ILogger logger) : base(vendrApi, settingsAccessor, uowProvider, logger)
#else
            ILogger<MethodSerializerBase<TObject>> logger) : base(vendrApi, settingsAccessor, uowProvider, logger)
#endif
        { }

        protected XElement SerializePrices(IReadOnlyList<ServicePrice> prices)
        {
            var root = new XElement("Prices");

            if (prices != null && prices.Any())
            {
                foreach (var price in prices)
                {
                    root.Add(new XElement("Price",
                        new XElement("Country", price.CountryId),
                        new XElement("Currency", price.CurrencyId),
                        new XElement("Region", price.RegionId),
                        new XElement("Value", price.Value)));
                }
            }
            return root;
        }

        protected List<SyncServicePriceModel> GetServicePrices(XElement node)
        {
            var prices = new List<SyncServicePriceModel>();

            var root = node.Element("Prices");
            if (root != null && root.HasElements)
            {
                foreach (var price in root.Elements("Price"))
                {
                    prices.Add(new SyncServicePriceModel
                    {
                        CountryId = price.GetGuidValue("Country"),
                        CurrencyId = price.GetGuidValue("Currency"),
                        RegionId = price.GetGuidValue("Region"),
                        Value = price.Element("Value").ValueOrDefault((decimal)0)
                    });
                }
            }

            return prices;

        }

        protected XElement SerializeCountryRegions(IReadOnlyList<AllowedCountryRegion> values)
        {
            var root = new XElement("AllowedCountryRegions");

            if (values != null && values.Any())
            {
                foreach (var value in values)
                {
                    root.Add(new XElement("Allowed",
                        new XElement("CountryId", value.CountryId),
                        new XElement("RegionId", value.RegionId)));
                }
            }

            return root;
        }

        protected List<SyncAllowedCountryRegionModel> GetCountryRegionsList(XElement node)
        {
            var countryRegions = new List<SyncAllowedCountryRegionModel>();

            // load the regions from the xml.
            var root = node.Element("AllowedCountryRegions");
            if (root != null && root.HasElements)
            {
                foreach (var value in root.Elements("Allowed"))
                {
                    var countryId = value.Element("CountryId").ValueOrDefault(Guid.Empty);
                    Guid? regionId = value.Element("RegionId").ValueOrDefault(Guid.Empty);
                    if (regionId == Guid.Empty) regionId = null;

                    countryRegions.Add(new SyncAllowedCountryRegionModel
                    {
                        CountryId = countryId,
                        RegionId = regionId
                    });
                }
            }

            return countryRegions;
        }
    }
}
