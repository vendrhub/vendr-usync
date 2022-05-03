using System;
using System.Xml.Linq;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common;

using Vendr.uSync.Extensions;
using Vendr.uSync.Configuration;

#if NETFRAMEWORK
using Umbraco.Core.Logging;
using uSync8.Core;
using uSync8.Core.Extensions;
using uSync8.Core.Models;
using uSync8.Core.Serialization;
#else
using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;
using Microsoft.Extensions.Logging;
#endif

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("62503EA1-6B7E-4567-92E2-9B67E2408434", "Region Serializer", VendrConstants.Serialization.Region)]
    public class RegionSerializer : VendrSerializerBase<RegionReadOnly>, ISyncSerializer<RegionReadOnly>
    {
        public RegionSerializer(IVendrApi vendrApi, VendrSyncSettingsAccessor settingsAccessor,
            IUnitOfWorkProvider uowProvider,
#if NETFRAMEWORK
            ILogger logger) : base(vendrApi, settingsAccessor, uowProvider, logger)
#else
            ILogger<RegionSerializer> logger) : base(vendrApi, settingsAccessor, uowProvider, logger)
#endif
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
                if (paymentMethodId != null && _vendrApi.GetPaymentMethod(paymentMethodId.Value) != null)
                {
                    item.SetDefaultPaymentMethod(paymentMethodId);
                }

                var shippingMethodId = node.GetGuidValue(nameof(item.DefaultShippingMethodId));
                if (shippingMethodId != null && _vendrApi.GetShippingMethod(shippingMethodId.Value) != null)
                {
                    item.SetDefaultShippingMethod(shippingMethodId);
                }

                _vendrApi.SaveRegion(item);

                uow.Complete();

                return SyncAttemptSucceed(name, item.AsReadOnly(), ChangeType.Import);
            }
        }

        public override string GetItemAlias(RegionReadOnly item)
            => item.Code;

        public override void DoDeleteItem(RegionReadOnly item)
            => _vendrApi.DeleteRegion(item.Id);

        public override RegionReadOnly DoFindItem(Guid key)
            => _vendrApi.GetRegion(key);

        public override void DoSaveItem(RegionReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);
                _vendrApi.SaveRegion(entity);
                uow.Complete();
            }
        }
    }
}
