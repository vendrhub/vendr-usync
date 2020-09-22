using System;
using System.Linq;
using System.Xml.Linq;

using Umbraco.Core.Logging;

using uSync8.Core;
using uSync8.Core.Extensions;
using uSync8.Core.Models;
using uSync8.Core.Serialization;

using Vendr.Core;
using Vendr.Core.Api;
using Vendr.Core.Models;

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("1C91B874-6028-4E50-AE1A-4481E9A267BD", "Shipping Method Serializer", VendrConstants.Serialization.ShippingMethod)]
    public class ShippingMethodSerializer : MethodSerializerBase<ShippingMethodReadOnly>, ISyncSerializer<ShippingMethodReadOnly>
    {
        public ShippingMethodSerializer(IVendrApi vendrApi, IUnitOfWorkProvider uowProvider, ILogger logger)
            : base(vendrApi, uowProvider, logger)
        { }

        protected override SyncAttempt<XElement> SerializeCore(ShippingMethodReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.Add(new XElement(nameof(item.StoreId), item.StoreId));

            node.Add(SerializeCountryRegions(item.AllowedCountryRegions));
            node.Add(SerializePrices(item.Prices));

            node.Add(new XElement(nameof(item.ImageId), item.ImageId));
            node.Add(new XElement(nameof(item.Sku), item.Sku));
            node.Add(new XElement(nameof(item.TaxClassId), item.TaxClassId));

            return SyncAttempt<XElement>.SucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        protected override SyncAttempt<ShippingMethodReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readonlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readonlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.Element(nameof(readonlyItem.StoreId)).ValueOrDefault(Guid.Empty);

            using (var uow = _uowProvider.Create())
            {
                ShippingMethod item;
                if (readonlyItem == null)
                {
                    item = ShippingMethod.Create(uow, id, storeId, alias, name);
                }
                else
                {
                    item = readonlyItem.AsWritable(uow);
                    item.SetName(name)
                        .SetAlias(alias);
                }

                item.SetSortOrder(node.Element(nameof(item.SortOrder)).ValueOrDefault(item.SortOrder));
                item.SetImage(node.Element(nameof(item.ImageId)).ValueOrDefault(item.ImageId));
                item.SetSku(node.Element(nameof(item.Sku)).ValueOrDefault(item.Sku));
                item.SetTaxClass(node.Element(nameof(item.TaxClassId)).ValueOrDefault(item.TaxClassId));

                DeserializeCountryRegions(node, item);

                DeserializePrices(node, item);

                _vendrApi.SaveShippingMethod(item);

                uow.Complete();

                return SyncAttempt<ShippingMethodReadOnly>.Succeed(name, item.AsReadOnly(), ChangeType.Import);

            }
        }

        private void DeserializeCountryRegions(XElement node, ShippingMethod item)
        {
            var countryRegions = GetCountryRegionsList(node);

            var valuesToRemove = item.AllowedCountryRegions
                .Where(x => countryRegions == null || !item.AllowedCountryRegions.Any(y => y.CountryId == x.CountryId
                     && y.RegionId == y.RegionId))
                .ToList();

            if (countryRegions.Count > 0)
            {
                foreach (var acr in countryRegions)
                {
                    if (acr.RegionId != null)
                    {
                        item.AllowInRegion(acr.CountryId, acr.RegionId.Value);
                    }
                    else
                    {
                        item.AllowInCountry(acr.CountryId);
                    }
                }
            }

            foreach (var acr in valuesToRemove)
            {
                if (acr.RegionId != null)
                {
                    item.DisallowInRegion(acr.CountryId, acr.RegionId.Value);
                }
                else
                {
                    item.DisallowInCountry(acr.CountryId);
                }
            }
        }

        private void DeserializePrices(XElement node, ShippingMethod item)
        {
            var prices = GetServicePrices(node);

            var pricesToRemove = item.Prices
                .Where(x => item.Prices == null
                || !prices.Any(y => y.CountryId == x.CountryId
                                && y.RegionId == x.RegionId
                                && y.CurrencyId == y.CurrencyId))
                .ToList();

            foreach (var price in prices)
            {
                if (price.CountryId == null && price.RegionId == null)
                {
                    item.SetDefaultPriceForCurrency(price.CurrencyId.Value, price.Value);
                }
                else
                {
                    if (price.RegionId != null)
                    {
                        item.SetRegionPriceForCurrency(price.CountryId.Value, price.RegionId.Value, price.CurrencyId.Value, price.Value);
                    }
                    else
                    {
                        item.SetCountryPriceForCurrency(price.CountryId.Value, price.CurrencyId.Value, price.Value);
                    }
                }
            }

            foreach (var price in pricesToRemove)
            {
                if (price.CountryId == null && price.RegionId == null)
                {
                    item.ClearDefaultPriceForCurrency(price.CurrencyId);
                }
                else if (price.CountryId != null && price.RegionId == null)
                {
                    item.ClearCountryPriceForCurrency(price.CountryId.Value, price.CurrencyId);
                }
                else
                {
                    item.ClearRegionPriceForCurrency(price.CountryId.Value, price.RegionId.Value, price.CurrencyId);
                }
            }
        }


        protected override void DeleteItem(ShippingMethodReadOnly item)
            => _vendrApi.DeleteShippingMethod(item.Id);

        protected override ShippingMethodReadOnly FindItem(Guid key)
            => _vendrApi.GetShippingMethod(key);

        protected override string ItemAlias(ShippingMethodReadOnly item)
            => item.Alias;

        protected override void SaveItem(ShippingMethodReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);
                _vendrApi.SaveShippingMethod(entity);
                uow.Complete();
            }
        }
    }
}
