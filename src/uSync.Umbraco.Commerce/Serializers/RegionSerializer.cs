using Microsoft.Extensions.Logging;
using System;
using System.Xml.Linq;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;
using uSync.Umbraco.Commerce.Configuration;
using uSync.Umbraco.Commerce.Extensions;

namespace uSync.Umbraco.Commerce.Serializers
{
    [SyncSerializer("62503EA1-6B7E-4567-92E2-9B67E2408434", "Region Serializer", CommerceConstants.Serialization.Region)]
    public class RegionSerializer : CommerceSerializerBase<RegionReadOnly>, ISyncSerializer<RegionReadOnly>
    {
        public RegionSerializer(ICommerceApi CommerceApi, CommerceSyncSettingsAccessor settingsAccessor,
            IUnitOfWorkProvider uowProvider,
            ILogger<RegionSerializer> logger) : base(CommerceApi, settingsAccessor, uowProvider, logger)
        { }

        protected override SyncAttempt<XElement> SerializeCore(RegionReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.AddStoreId(item.StoreId);

            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));

            node.Add(new XElement(nameof(item.Code), item.Code));
            node.Add(new XElement(nameof(item.CountryId), item.CountryId));
            node.Add(new XElement(nameof(item.DefaultPaymentMethodId), item.DefaultPaymentMethodId));
            node.Add(new XElement(nameof(item.DefaultShippingMethodId), item.DefaultShippingMethodId));

            return SyncAttemptSucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        public override bool IsValid(XElement node)
            => base.IsValid(node)
                && node.GetStoreId() != Guid.Empty
                && node.Element("CountryId").ValueOrDefault(Guid.Empty) != Guid.Empty;

        protected override SyncAttempt<RegionReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readonlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readonlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.GetStoreId();
            var countryId = node.Element(nameof(readonlyItem.CountryId)).ValueOrDefault(Guid.Empty);

            var code = node.Element(nameof(readonlyItem.Code)).ValueOrDefault(string.Empty);

            if (storeId == Guid.Empty || countryId == Guid.Empty)
            {
                // fail
            }

            using (var uow = _uowProvider.Create())
            {
                Region item;
                if (readonlyItem == null)
                {
                    item = Region.Create(uow, id, storeId, countryId, code, name);
                }
                else
                {
                    item = readonlyItem.AsWritable(uow);
                    item.SetCode(code)
                        .SetName(name);
                }

                item.SetSortOrder(node.Element(nameof(item.SortOrder)).ValueOrDefault(item.SortOrder));

                var paymentMethodId = node.GetGuidValue(nameof(item.DefaultPaymentMethodId));
                if (paymentMethodId != null && _CommerceApi.GetPaymentMethod(paymentMethodId.Value) != null)
                {
                    item.SetDefaultPaymentMethod(paymentMethodId);
                }

                var shippingMethodId = node.GetGuidValue(nameof(item.DefaultShippingMethodId));
                if (shippingMethodId != null && _CommerceApi.GetShippingMethod(shippingMethodId.Value) != null)
                {
                    item.SetDefaultShippingMethod(shippingMethodId);
                }

                _CommerceApi.SaveRegion(item);

                uow.Complete();

                return SyncAttemptSucceed(name, item.AsReadOnly(), ChangeType.Import);
            }
        }

        public override string GetItemAlias(RegionReadOnly item)
            => item.Code;

        public override void DoDeleteItem(RegionReadOnly item)
            => _CommerceApi.DeleteRegion(item.Id);

        public override RegionReadOnly DoFindItem(Guid key)
            => _CommerceApi.GetRegion(key);

        public override void DoSaveItem(RegionReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);
                _CommerceApi.SaveRegion(entity);
                uow.Complete();
            }
        }
    }
}
