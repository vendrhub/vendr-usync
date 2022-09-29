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
    [SyncHandler("vendrRegionHandler", "Regions", "Vendr\\Region", VendrConstants.Priorites.Region,
        Icon = "icon-flag-alt", IsTwoPass = true, EntityType = VendrConstants.UdiEntityType.Region)]
    public class RegionHandler : VendrSyncHandlerBase<RegionReadOnly>, ISyncVendrHandler, ISyncPostImportHandler
        , IEventHandlerFor<RegionSavedNotification>
        , IEventHandlerFor<RegionDeletedNotification>
    {
        public RegionHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<RegionReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(RegionReadOnly item)
            => _vendrApi.DeleteRegion(item.Id);

        protected override IEnumerable<RegionReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetRegions(storeId);

        protected override RegionReadOnly GetFromService(Guid key)
            => _vendrApi.GetRegion(key);

        protected override string GetItemName(RegionReadOnly item)
            => item.Name;

        public void Handle(RegionSavedNotification notification)
            => VendrItemSaved(notification.Region);

        public void Handle(RegionDeletedNotification notification)
            => VendrItemDeleted(notification.Region);
    }
}
