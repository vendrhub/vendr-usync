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
    [SyncHandler("CommerceRegionHandler", "Regions", "Commerce\\Region", CommerceConstants.Priorites.Region,
        Icon = "icon-flag-alt", IsTwoPass = true, EntityType = CommerceConstants.UdiEntityType.Region)]
    public class RegionHandler : CommerceSyncHandlerBase<RegionReadOnly>, ISyncHandler, ISyncPostImportHandler
        , IEventHandlerFor<RegionSavedNotification>
        , IEventHandlerFor<RegionDeletedNotification>
    {

        public RegionHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<RegionReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(RegionReadOnly item)
            => _CommerceApi.DeleteRegion(item.Id);

        protected override IEnumerable<RegionReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetRegions(storeId);

        protected override RegionReadOnly GetFromService(Guid key)
            => _CommerceApi.GetRegion(key);

        protected override string GetItemName(RegionReadOnly item)
            => item.Name;

        public void Handle(RegionSavedNotification notification)
            => CommerceItemSaved(notification.Region);

        public void Handle(RegionDeletedNotification notification)
            => CommerceItemDeleted(notification.Region);
    }
}
