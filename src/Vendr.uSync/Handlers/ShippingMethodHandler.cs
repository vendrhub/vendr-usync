using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Vendr.Core.Api;
using Vendr.Core.Events.Notification;
using Vendr.Core.Models;
using Vendr.Common.Events;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;

using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrShippingMethodHandler", "Shipping Methods", "Vendr\\ShippingMethod", VendrConstants.Priorites.ShippingMethod,
        Icon = "icon-truck", EntityType = VendrConstants.UdiEntityType.ShippingMethod)]
    public class ShippingMethodHandler : VendrSyncHandlerBase<ShippingMethodReadOnly>, ISyncVendrHandler
        , IEventHandlerFor<ShippingMethodSavedNotification>
        , IEventHandlerFor<ShippingMethodDeletedNotification>
    {
        public ShippingMethodHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<ShippingMethodReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) 
            : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override IEnumerable<ShippingMethodReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetShippingMethods(storeId);

        protected override void DeleteViaService(ShippingMethodReadOnly item)
            => _vendrApi.DeleteShippingMethod(item.Id);

        protected override ShippingMethodReadOnly GetFromService(Guid key)
            => _vendrApi.GetShippingMethod(key);

        protected override string GetItemName(ShippingMethodReadOnly item)
            => item.Name;

        public void Handler(ShippingMethodSavedNotification notification)
            => VendrItemSaved(notification.ShippingMethod);

        public void Handler(ShippingMethodDeletedNotification notification)
            => VendrItemDeleted(notification.ShippingMethod);
    }
}
