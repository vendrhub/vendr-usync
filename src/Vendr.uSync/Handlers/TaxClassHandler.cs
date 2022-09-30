using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common.Events;
using Vendr.Core.Events.Notification;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;


namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrTaxClassHandler", "Taxes", "Vendr\\TaxClass", VendrConstants.Priorites.TaxClass,
        Icon = "icon-library", EntityType = VendrConstants.UdiEntityType.TaxClass)]
    public class TaxClassHandler : VendrSyncHandlerBase<TaxClassReadOnly>, ISyncVendrHandler
        , IEventHandlerFor<TaxClassSavedNotification>
        , IEventHandlerFor<TaxClassDeletedNotification>
    {
        public TaxClassHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<TaxClassReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory)
            : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory) { }

        protected override TaxClassReadOnly GetFromService(Guid key)
            => _vendrApi.GetTaxClass(key);
        protected override void DeleteViaService(TaxClassReadOnly item)
            => _vendrApi.DeleteTaxClass(item.Id);

        protected override string GetItemName(TaxClassReadOnly item)
            => item.Name;

        protected override IEnumerable<TaxClassReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetTaxClasses(storeId);

        public void Handle(TaxClassSavedNotification notification)
            => VendrItemSaved(notification.TaxClass);

        public void Handle(TaxClassDeletedNotification notification)
            => VendrItemDeleted(notification.TaxClass);
    }
}
