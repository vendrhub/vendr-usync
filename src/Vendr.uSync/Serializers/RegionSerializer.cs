using System;
using System.Xml.Linq;

using Umbraco.Core.Logging;

using uSync8.Core;
using uSync8.Core.Extensions;
using uSync8.Core.Models;
using uSync8.Core.Serialization;

using Vendr.Core;
using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.uSync.Extensions;

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("62503EA1-6B7E-4567-92E2-9B67E2408434", "Region Serializer", VendrConstants.Serialization.Region)]
    public class RegionSerializer : VendrSerializerBase<RegionReadOnly>, ISyncSerializer<RegionReadOnly>
    {
        public RegionSerializer(IVendrApi vendrApi, IUnitOfWorkProvider uowProvider, ILogger logger) : base(vendrApi, uowProvider, logger)
        {
        }

        protected override SyncAttempt<XElement> SerializeCore(RegionReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.Add(new XElement(nameof(item.StoreId), item.StoreId));

            node.Add(new XElement(nameof(item.Code), item.Code));
            node.Add(new XElement(nameof(item.CountryId), item.CountryId));
            node.Add(new XElement(nameof(item.DefaultPaymentMethodId), item.DefaultPaymentMethodId));
            node.Add(new XElement(nameof(item.DefaultShippingMethodId), item.DefaultShippingMethodId));

            return SyncAttempt<XElement>.SucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        protected override SyncAttempt<RegionReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readonlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readonlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.Element(nameof(readonlyItem.StoreId)).ValueOrDefault(Guid.Empty);
            var countryId = node.Element(nameof(readonlyItem.CountryId)).ValueOrDefault(Guid.Empty);

            var code = node.Element(nameof(readonlyItem.Code)).ValueOrDefault(readonlyItem.Code);

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
                item.SetDefaultPaymentMethod(node.GetGuidValue(nameof(item.DefaultPaymentMethodId)));
                item.SetDefaultShippingMethod(node.GetGuidValue(nameof(item.DefaultShippingMethodId)));

                _vendrApi.SaveRegion(item);

                uow.Complete();

                return SyncAttempt<RegionReadOnly>.Succeed(name, item.AsReadOnly(), ChangeType.Import);
            }
        }

        protected override void DeleteItem(RegionReadOnly item)
            => _vendrApi.DeleteRegion(item.Id);

        protected override RegionReadOnly FindItem(Guid key)
            => _vendrApi.GetRegion(key);

        protected override string ItemAlias(RegionReadOnly item)
            => item.Code;

        protected override void SaveItem(RegionReadOnly item)
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
