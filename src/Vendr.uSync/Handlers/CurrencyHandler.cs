using System;
using System.Collections.Generic;

using Vendr.Common.Events;
using Vendr.Core.Api;
using Vendr.Core.Events.Notification;
using Vendr.Core.Models;

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
    [SyncHandler("vendrCurrencyHandler", "Currencies", "Vendr\\Currency", VendrConstants.Priorites.Currency,
        Icon = "icon-coins-dollar-alt", EntityType = VendrConstants.UdiEntityType.Currency)]
    public class CurrencyHandler : VendrSyncHandlerBase<CurrencyReadOnly>, ISyncVendrHandler
    {
#if NETFRAMEWORK
        public CurrencyHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<CurrencyReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }
#else
        public CurrencyHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<CurrencyReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }
#endif

        protected override void DeleteViaService(CurrencyReadOnly item)
            => _vendrApi.DeleteCurrency(item.Id);

        protected override IEnumerable<CurrencyReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetCurrencies(storeId);

        protected override CurrencyReadOnly GetFromService(Guid key)
            => _vendrApi.GetCurrency(key);

        protected override string GetItemName(CurrencyReadOnly item)
            => item.Name;

        public void Handle(IEvent evt)
        {
            switch (evt)
            {
                case CurrencySavedNotification savedNotification:
                    VendrItemSaved(savedNotification.Currency);
                    break;
                case CurrencyDeletedNotification deletedNotification:
                    VendrItemDeleted(deletedNotification.Currency);
                    break;
            }
        }

    }
}
