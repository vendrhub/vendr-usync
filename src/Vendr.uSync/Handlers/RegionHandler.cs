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
    [SyncHandler("vendrRegionHandler", "Regions", "Vendr\\Region", VendrConstants.Priorites.Region,
        Icon = "icon-flag-alt", IsTwoPass = true, EntityType = VendrConstants.UdiEntityType.Region)]
    public class RegionHandler : VendrSyncHandlerBase<RegionReadOnly>, ISyncVendrHandler, ISyncPostImportHandler
        , IEventHandlerFor<RegionSavedNotification>
        , IEventHandlerFor<RegionDeletedNotification>
    {

#if NETFRAMEWORK
        public RegionHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<RegionReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService)
            : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }
#else
        public RegionHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<RegionReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }
#endif

        protected override void DeleteViaService(RegionReadOnly item)
            => _vendrApi.DeleteRegion(item.Id);

        protected override IEnumerable<RegionReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetRegions(storeId);

        protected override RegionReadOnly GetFromService(Guid key)
            => _vendrApi.GetRegion(key);

        protected override string GetItemName(RegionReadOnly item)
            => item.Name;

        public void Handle(IEvent evt)
        {
            switch (evt)
            {
                case RegionSavedNotification savedNotification:
                    VendrItemSaved(savedNotification.Region);
                    break;
                case RegionDeletedNotification deletedNotification:
                    VendrItemDeleted(deletedNotification.Region);
                    break;
            }
        }
    }
}
