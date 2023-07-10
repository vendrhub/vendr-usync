using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Events.Notification;
using Umbraco.Commerce.Core.Models;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;


namespace uSync.Umbraco.Commerce.Handlers
{
    [SyncHandler("CommerceShippingMethodHandler", "Shipping Methods", "Commerce\\ShippingMethod", CommerceConstants.Priorites.ShippingMethod,
        Icon = "icon-truck", EntityType = CommerceConstants.UdiEntityType.ShippingMethod)]
    public class ShippingMethodHandler : CommerceSyncHandlerBase<ShippingMethodReadOnly>, ISyncHandler
        , IEventHandlerFor<ShippingMethodSavedNotification>
        , IEventHandlerFor<ShippingMethodDeletedNotification>
    {

        public ShippingMethodHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<ShippingMethodReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory)
            : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override IEnumerable<ShippingMethodReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetShippingMethods(storeId);

        protected override void DeleteViaService(ShippingMethodReadOnly item)
            => _CommerceApi.DeleteShippingMethod(item.Id);

        protected override ShippingMethodReadOnly GetFromService(Guid key)
            => _CommerceApi.GetShippingMethod(key);

        protected override string GetItemName(ShippingMethodReadOnly item)
            => item.Name;

        public void Handler(ShippingMethodSavedNotification notification)
            => CommerceItemSaved(notification.ShippingMethod);

        public void Handler(ShippingMethodDeletedNotification notification)
            => CommerceItemDeleted(notification.ShippingMethod);
    }
}
