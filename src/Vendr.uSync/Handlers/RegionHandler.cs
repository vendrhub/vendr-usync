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
    [SyncHandler("vendrRegionHandler", "Regions", "vendr\\Region", VendrConstants.Priorites.Region,
        Icon = "icon-flag-alt")]
    public class RegionHandler : VendrSyncHandlerBase<RegionReadOnly>, ISyncExtendedHandler
    {
        public override string Group => VendrConstants.Group;

        public RegionHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<RegionReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService)
            : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }

        protected override void DeleteViaService(RegionReadOnly item)
            => _vendrApi.DeleteRegion(item.Id);

        protected override IEnumerable<RegionReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetRegions(storeId);

        protected override RegionReadOnly GetFromService(Guid key)
            => _vendrApi.GetRegion(key);

        protected override string GetItemName(RegionReadOnly item)
            => item.Name;

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnRegionSaved((e) => VendrItemSaved(e.Region));
            EventHub.NotificationEvents.OnRegionDeleted((e) => VendrItemDeleted(e.Region));
        }
    }
}
