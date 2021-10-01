using System;
using System.Xml.Linq;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common;

using Vendr.uSync.Extensions;

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
    [SyncSerializer("FA15B3E1-8100-431E-BC95-4B74134A42DD", "OrderStatus Serializer", VendrConstants.Serialization.OrderStatus)]
    public class OrderStatusSerializer : VendrSerializerBase<OrderStatusReadOnly>, ISyncSerializer<OrderStatusReadOnly>
#if NETFRAMEWORK
        , ISyncNodeSerializer<OrderStatusReadOnly>
#endif
    {
        public OrderStatusSerializer(IVendrApi vendrApi, 
            IUnitOfWorkProvider uowProvider,
#if NETFRAMEWORK
            ILogger logger) : base(vendrApi, uowProvider, logger)
#else
            ILogger<OrderStatusSerializer> logger) : base(vendrApi, uowProvider, logger)
#endif
        { }

        protected override SyncAttempt<XElement> SerializeCore(OrderStatusReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.AddStoreId(item.StoreId);

            node.Add(new XElement(nameof(item.Color), item.Color));

            return SyncAttemptSucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        public override bool IsValid(XElement node)
            => base.IsValid(node)
            && node.GetStoreId() != Guid.Empty;

        protected override SyncAttempt<OrderStatusReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readonlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readonlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.GetStoreId();

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

                return SyncAttemptSucceed(name, item.AsReadOnly(), ChangeType.Import);
            }
        }

        public override string GetItemAlias(OrderStatusReadOnly item)
            => item.Alias;

        public override void DoDeleteItem(OrderStatusReadOnly item)
            => _vendrApi.DeleteOrderStatus(item.Id);

        public override OrderStatusReadOnly DoFindItem(Guid key)
            => _vendrApi.GetOrderStatus(key);

        public override void DoSaveItem(OrderStatusReadOnly item)
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
