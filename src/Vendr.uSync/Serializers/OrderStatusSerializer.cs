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

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("FA15B3E1-8100-431E-BC95-4B74134A42DD", "OrderStatus Serializer", VendrConstants.Serialization.OrderStatus)]
    public class OrderStatusSerializer : VendrSerializerBase<OrderStatusReadOnly>, ISyncSerializer<OrderStatusReadOnly>
    {
        public OrderStatusSerializer(IVendrApi vendrApi, IUnitOfWorkProvider uowProvider, ILogger logger)
            : base(vendrApi, uowProvider, logger)
        { }

        protected override SyncAttempt<XElement> SerializeCore(OrderStatusReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.Add(new XElement(nameof(item.StoreId), item.StoreId));

            node.Add(new XElement(nameof(item.Color), item.Color));

            return SyncAttempt<XElement>.SucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        protected override SyncAttempt<OrderStatusReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readonlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readonlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.Element(nameof(readonlyItem.StoreId)).ValueOrDefault(Guid.Empty);

            using (var uow = _uowProvider.Create())
            {
                OrderStatus item;
                if (readonlyItem == null)
                {
                    item = OrderStatus.Create(uow, id, storeId, alias, name);
                }
                else
                {
                    item = readonlyItem.AsWritable(uow);
                    item.SetAlias(alias)
                        .SetName(name);
                }

                item.SetColor(node.Element(nameof(item.Color)).ValueOrDefault(item.Color));
                item.SetSortOrder(node.Element(nameof(item.SortOrder)).ValueOrDefault(item.SortOrder));

                _vendrApi.SaveOrderStatus(item);

                uow.Complete();

                return SyncAttempt<OrderStatusReadOnly>.Succeed(name, item.AsReadOnly(), ChangeType.Import);
            }
        }

        protected override void DeleteItem(OrderStatusReadOnly item)
            => _vendrApi.DeleteOrderStatus(item.Id);

        protected override OrderStatusReadOnly FindItem(Guid key)
            => _vendrApi.GetOrderStatus(key);

        protected override string ItemAlias(OrderStatusReadOnly item)
            => item.Alias;

        protected override void SaveItem(OrderStatusReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);
                _vendrApi.SaveOrderStatus(entity);
                uow.Complete();
            }
        }
    }
}
