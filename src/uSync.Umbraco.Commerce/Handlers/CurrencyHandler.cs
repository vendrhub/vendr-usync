using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Events.Notification;
using Umbraco.Commerce.Core.Models;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

namespace uSync.Umbraco.Commerce.Handlers
{
    [SyncHandler("CommerceCurrencyHandler", "Currencies", "Commerce\\Currency", CommerceConstants.Priorites.Currency,
        Icon = "icon-coins-dollar-alt", EntityType = CommerceConstants.UdiEntityType.Currency)]
    public class CurrencyHandler : CommerceSyncHandlerBase<CurrencyReadOnly>, ISyncHandler
    {
        public CurrencyHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<CurrencyReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(CurrencyReadOnly item)
            => _CommerceApi.DeleteCurrency(item.Id);

        protected override IEnumerable<CurrencyReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetCurrencies(storeId);

        protected override CurrencyReadOnly GetFromService(Guid key)
            => _CommerceApi.GetCurrency(key);

        protected override string GetItemName(CurrencyReadOnly item)
            => item.Name;


        public void Handle(CurrencySavedNotification notification)
            => CommerceItemSaved(notification.Currency);

        public void Handle(CurrencyDeletedNotification notification)
            => CommerceItemDeleted(notification.Currency);
    }
}
