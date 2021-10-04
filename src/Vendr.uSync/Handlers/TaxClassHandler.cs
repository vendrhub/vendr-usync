using System;
using System.Collections.Generic;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common.Events;
using Vendr.Core.Events.Notification;

#if NETFRAMEWORK
using uSync8.BackOffice.Services;
using uSync8.BackOffice.SyncHandlers;
using uSync8.Core;
using uSync8.Core.Serialization;

using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
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
    [SyncHandler("vendrTaxClassHandler", "Taxes", "Vendr\\TaxClass", VendrConstants.Priorites.TaxClass,
        Icon = "icon-library", EntityType = VendrConstants.UdiEntityType.TaxClass)]
    public class TaxClassHandler : VendrSyncHandlerBase<TaxClassReadOnly>, ISyncVendrHandler
        , IEventHandlerFor<TaxClassSavedNotification>
        , IEventHandlerFor<TaxClassDeletedNotification>
    {

#if NETFRAMEWORK
        public TaxClassHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<TaxClassReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService)
            : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService) { }
#else
        public TaxClassHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<TaxClassReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory)
            : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory) { }
#endif

        protected override TaxClassReadOnly GetFromService(Guid key)
            => _vendrApi.GetTaxClass(key);
        protected override void DeleteViaService(TaxClassReadOnly item)
            => _vendrApi.DeleteTaxClass(item.Id);

        protected override string GetItemName(TaxClassReadOnly item)
            => item.Name;

        protected override IEnumerable<TaxClassReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetTaxClasses(storeId);

        public void Handle(IEvent evt)
        {
            switch (evt)
            {
                case TaxClassSavedNotification savedNotification:
                    VendrItemSaved(savedNotification.TaxClass);
                    break;
                case TaxClassDeletedNotification deletedNotification:
                    VendrItemDeleted(deletedNotification.TaxClass);
                    break;
            }
        }
    }
}
