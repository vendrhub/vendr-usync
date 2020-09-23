using System;
using System.Collections.Generic;

using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

using uSync8.BackOffice.Configuration;
using uSync8.BackOffice.Services;
using uSync8.BackOffice.SyncHandlers;
using uSync8.Core;
using uSync8.Core.Serialization;

using Vendr.Core.Api;
using Vendr.Core.Events;
using Vendr.Core.Models;

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrOrderStatusHandler", "Order Statuses", "Vendr\\OrderStatus", VendrConstants.Priorites.OrderStatus,
        Icon = "icon-file-cabinet")]
    public class OrderStatusHandler : VendrSyncHandlerBase<OrderStatusReadOnly>, ISyncExtendedHandler
    {
        public override string Group => VendrConstants.Group;

        public OrderStatusHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<OrderStatusReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        {
        }

        protected override void DeleteViaService(OrderStatusReadOnly item)
            => _vendrApi.DeleteOrderStatus(item.Id);

        protected override IEnumerable<OrderStatusReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetOrderStatuses(storeId);

        protected override OrderStatusReadOnly GetFromService(Guid key)
            => _vendrApi.GetOrderStatus(key);

        protected override string GetItemName(OrderStatusReadOnly item)
            => item.Name;

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnOrderStatusSaved((e) => VendrItemSaved(e.OrderStatus));
            EventHub.NotificationEvents.OnOrderStatusDeleted((e) => VendrItemDeleted(e.OrderStatus));
        }
    }
}
