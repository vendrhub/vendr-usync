using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Xml.Linq;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using Umbraco.Extensions;
using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;
using uSync.Umbraco.Commerce.Configuration;
using uSync.Umbraco.Commerce.Extensions;

namespace uSync.Umbraco.Commerce.Serializers
{
    [SyncSerializer("1C91B874-6028-4E50-AE1A-4481E9A267BD", "Shipping Method Serializer", CommerceConstants.Serialization.ShippingMethod)]
    public class ShippingMethodSerializer : MethodSerializerBase<ShippingMethodReadOnly>, ISyncSerializer<ShippingMethodReadOnly>
    {
        public ShippingMethodSerializer(ICommerceApi CommerceApi, CommerceSyncSettingsAccessor settingsAccessor,
            IUnitOfWorkProvider uowProvider,
            ILogger<ShippingMethodSerializer> logger) : base(CommerceApi, settingsAccessor, uowProvider, logger)
        { }

        protected override SyncAttempt<XElement> SerializeCore(ShippingMethodReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));


            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));

            node.AddStoreId(item.StoreId);

            node.Add(SerializeCountryRegions(item.AllowedCountryRegions));
            node.Add(SerializePrices(item.Prices));

            node.Add(new XElement(nameof(item.ImageId), item.ImageId));
            node.Add(new XElement(nameof(item.Sku), item.Sku));
            node.Add(new XElement(nameof(item.TaxClassId), item.TaxClassId));

            return SyncAttemptSucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        public override bool IsValid(XElement node)
            => base.IsValid(node)
            && node.GetStoreId() != Guid.Empty;

        protected override SyncAttempt<ShippingMethodReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readonlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readonlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.GetStoreId();

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

                _CommerceApi.SaveShippingMethod(item);

                uow.Complete();

                return SyncAttemptSucceed(name, item.AsReadOnly(), ChangeType.Import);

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

        public override string GetItemAlias(ShippingMethodReadOnly item)
            => item.Alias;

        public override void DoDeleteItem(ShippingMethodReadOnly item)
            => _CommerceApi.DeleteShippingMethod(item.Id);

        public override ShippingMethodReadOnly DoFindItem(Guid key)
            => _CommerceApi.GetShippingMethod(key);

        public override void DoSaveItem(ShippingMethodReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);
                _CommerceApi.SaveShippingMethod(entity);
                uow.Complete();
            }
        }
    }
}
