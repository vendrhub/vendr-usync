using System;
using System.Collections.Generic;

using Vendr.Core.Api;
using Vendr.Core.Events.Notification;
using Vendr.Core.Models;
using Vendr.Common.Events;

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
    [SyncHandler("vendrShippingMethodHandler", "Shipping Methods", "Vendr\\ShippingMethod", VendrConstants.Priorites.ShippingMethod,
        Icon = "icon-truck", EntityType = VendrConstants.UdiEntityType.ShippingMethod)]
    public class ShippingMethodHandler : VendrSyncHandlerBase<ShippingMethodReadOnly>, ISyncVendrHandler
        , IEventHandlerFor<ShippingMethodSavedNotification>
        , IEventHandlerFor<ShippingMethodDeletedNotification>
    {

#if NETFRAMEWORK
        public ShippingMethodHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<ShippingMethodReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService)
            : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }
#else
        public ShippingMethodHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<ShippingMethodReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) 
            : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }
#endif

        protected override IEnumerable<ShippingMethodReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetShippingMethods(storeId);

        protected override void DeleteViaService(ShippingMethodReadOnly item)
            => _vendrApi.DeleteShippingMethod(item.Id);

        protected override ShippingMethodReadOnly GetFromService(Guid key)
            => _vendrApi.GetShippingMethod(key);

        protected override string GetItemName(ShippingMethodReadOnly item)
            => item.Name;

        public void Handle(IEvent evt)
        {
            switch (evt)
            {
                case ShippingMethodSavedNotification savedNotification:
                    VendrItemSaved(savedNotification.ShippingMethod);
                    break;
                case ShippingMethodDeletedNotification deletedNotification:
                    VendrItemDeleted(deletedNotification.ShippingMethod);
                    break;
            }
        }
    }
}
