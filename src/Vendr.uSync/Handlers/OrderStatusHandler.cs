using System;
using System.Collections.Generic;

using Vendr.Common.Events;
using Vendr.Core.Api;
using Vendr.Core.Events.Notification;
using Vendr.Core.Models;

#if NETFRAMEWORK
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

using uSync8.BackOffice.Services;
using uSync8.BackOffice.SyncHandlers;
using uSync8.Core;
using uSync8.Core.Serialization;
#else 
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;

using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;
#endif

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrOrderStatusHandler", "Order Statuses", "Vendr\\OrderStatus", VendrConstants.Priorites.OrderStatus,
        Icon = "icon-file-cabinet", EntityType = VendrConstants.UdiEntityType.OrderStatus)]
    public class OrderStatusHandler : VendrSyncHandlerBase<OrderStatusReadOnly>, ISyncVendrHandler
    {
#if NETFRAMEWORK
        public OrderStatusHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<OrderStatusReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }
#else
        public OrderStatusHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<OrderStatusReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }
#endif

        protected override void DeleteViaService(OrderStatusReadOnly item)
            => _vendrApi.DeleteOrderStatus(item.Id);

        protected override IEnumerable<OrderStatusReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetOrderStatuses(storeId);

        protected override OrderStatusReadOnly GetFromService(Guid key)
            => _vendrApi.GetOrderStatus(key);

        protected override string GetItemName(OrderStatusReadOnly item)
            => item.Name;


        public void Handle(OrderStatusSavedNotification notification)
            => VendrItemSaved(notification.OrderStatus);

        public void Handle(OrderStatusDeletedNotification notification)
            => VendrItemDeleted(notification.OrderStatus);
    }
}
