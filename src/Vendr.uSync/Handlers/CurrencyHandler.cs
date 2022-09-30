using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Vendr.Core.Api;
using Vendr.Core.Events.Notification;
using Vendr.Core.Models;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;

using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrCurrencyHandler", "Currencies", "Vendr\\Currency", VendrConstants.Priorites.Currency,
        Icon = "icon-coins-dollar-alt", EntityType = VendrConstants.UdiEntityType.Currency)]
    public class CurrencyHandler : VendrSyncHandlerBase<CurrencyReadOnly>, ISyncVendrHandler
    {
        public CurrencyHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<CurrencyReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(CurrencyReadOnly item)
            => _vendrApi.DeleteCurrency(item.Id);

        protected override IEnumerable<CurrencyReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetCurrencies(storeId);

        protected override CurrencyReadOnly GetFromService(Guid key)
            => _vendrApi.GetCurrency(key);

        protected override string GetItemName(CurrencyReadOnly item)
            => item.Name;


        public void Handle(CurrencySavedNotification notification)
            => VendrItemSaved(notification.Currency);

        public void Handle(CurrencyDeletedNotification notification)
            => VendrItemDeleted(notification.Currency);
    }
}
