using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Events.Notification;
using Umbraco.Commerce.Core.Models;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

namespace uSync.Umbraco.Commerce.Handlers
{
    [SyncHandler("CommerceOrderStatusHandler", "Order Statuses", "Commerce\\OrderStatus", CommerceConstants.Priorites.OrderStatus,
        Icon = "icon-file-cabinet", EntityType = CommerceConstants.UdiEntityType.OrderStatus)]
    public class OrderStatusHandler : CommerceSyncHandlerBase<OrderStatusReadOnly>, ISyncHandler
    {
        public OrderStatusHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<OrderStatusReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(OrderStatusReadOnly item)
            => _CommerceApi.DeleteOrderStatus(item.Id);

        protected override IEnumerable<OrderStatusReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetOrderStatuses(storeId);

        protected override OrderStatusReadOnly GetFromService(Guid key)
            => _CommerceApi.GetOrderStatus(key);

        protected override string GetItemName(OrderStatusReadOnly item)
            => item.Name;


        public void Handle(OrderStatusSavedNotification notification)
            => CommerceItemSaved(notification.OrderStatus);

        public void Handle(OrderStatusDeletedNotification notification)
            => CommerceItemDeleted(notification.OrderStatus);
    }
}
