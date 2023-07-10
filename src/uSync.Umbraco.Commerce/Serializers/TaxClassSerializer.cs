using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
using uSync.Umbraco.Commerce.SyncModels;

namespace uSync.Umbraco.Commerce.Serializers
{
    [SyncSerializer("22F98052-DD59-4A0C-AA13-52398B794ED5", "TaxClass Serializer", CommerceConstants.Serialization.TaxClass)]
    public class TaxClassSerializer : CommerceSerializerBase<TaxClassReadOnly>, ISyncSerializer<TaxClassReadOnly>
    {
        public TaxClassSerializer(ICommerceApi CommerceApi, CommerceSyncSettingsAccessor settingsAccessor,
            IUnitOfWorkProvider uowProvider,
            ILogger<TaxClassSerializer> logger) : base(CommerceApi, settingsAccessor, uowProvider, logger)
        { }

        protected override SyncAttempt<XElement> SerializeCore(TaxClassReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.AddStoreId(item.StoreId);

            node.Add(new XElement(nameof(item.DefaultTaxRate), item.DefaultTaxRate.Value));

            node.Add(SerializeTaxRates(item));

            return SyncAttemptSucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        private XElement SerializeTaxRates(TaxClassReadOnly item)
        {
            var root = new XElement("TaxRates");

            foreach (var rate in item.CountryRegionTaxRates)
            {
                root.Add(new XElement("Rate",
                    new XElement("CountryId", rate.CountryId),
                    new XElement("RegionId", rate.RegionId),
                    new XElement("TaxRate", rate.TaxRate)));
            }

            return root;
        }


        public override bool IsValid(XElement node)
            => base.IsValid(node)
            && node.GetStoreId() != Guid.Empty;

        protected override SyncAttempt<TaxClassReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readonlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readonlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.GetStoreId();
            var defaultTaxRate = node.Element(nameof(readonlyItem.DefaultTaxRate)).ValueOrDefault((decimal)0);

            using (var uow = _uowProvider.Create())
            {
                TaxClass item;
                if (readonlyItem == null)
                {
                    item = TaxClass.Create(uow, id, storeId, alias, name, defaultTaxRate);
                }
                else
                {
                    item = readonlyItem.AsWritable(uow);
                    item.SetAlias(alias)
                        .SetName(name)
                        .SetDefaultTaxRate(defaultTaxRate);
                }

                item.SetSortOrder(node.Element(nameof(item.SortOrder)).ValueOrDefault(item.SortOrder));

                DeserializeTaxRates(node, item);

                _CommerceApi.SaveTaxClass(item);

                uow.Complete();

                return SyncAttemptSucceed(name, item.AsReadOnly(), ChangeType.Import);
            }
        }

        protected List<SyncTaxRateModel> GetTaxRates(XElement node)
        {
            var taxRates = new List<SyncTaxRateModel>();

            // load the regions from the xml.
            var root = node.Element("TaxRates");
            if (root != null && root.HasElements)
            {
                foreach (var value in root.Elements("Rate"))
                {
                    taxRates.Add(new SyncTaxRateModel
                    {
                        CountryId = value.GetGuidValue("CountryId"),
                        RegionId = value.GetGuidValue("RegionId"),
                        Rate = value.Element("TaxRate").ValueOrDefault((decimal)0)
                    });
                }
            }

            return taxRates;
        }

        protected void DeserializeTaxRates(XElement node, TaxClass item)
        {
            var rates = GetTaxRates(node);

            var ratesToRemove = item.CountryRegionTaxRates
                .Where(x => rates == null || !rates.Any(y => y.CountryId == x.CountryId && y.RegionId == x.RegionId))
                .ToList();

            foreach (var rate in rates)
            {
                if (rate.RegionId == null)
                {
                    item.SetCountryTaxRate(rate.CountryId.Value, rate.Rate);
                }
                else
                {
                    item.SetRegionTaxRate(rate.CountryId.Value, rate.RegionId.Value, rate.Rate);
                }
            }

            foreach (var rate in ratesToRemove)
            {
                if (rate.RegionId == null)
                {
                    item.ClearCountryTaxRate(rate.CountryId);
                }
                else
                {
                    item.ClearRegionTaxRate(rate.CountryId, rate.RegionId.Value);
                }
            }
        }

        public override string GetItemAlias(TaxClassReadOnly item)
            => item.Alias;

        public override void DoDeleteItem(TaxClassReadOnly item)
            => _CommerceApi.DeleteTaxClass(item.Id);

        public override TaxClassReadOnly DoFindItem(Guid key)
            => _CommerceApi.GetTaxClass(key);

        public override void DoSaveItem(TaxClassReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);
                _CommerceApi.SaveTaxClass(entity);
                uow.Complete();
            }
        }
    }
}
