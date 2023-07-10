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
    [SyncHandler("CommerceTaxClassHandler", "Taxes", "Commerce\\TaxClass", CommerceConstants.Priorites.TaxClass,
        Icon = "icon-library", EntityType = CommerceConstants.UdiEntityType.TaxClass)]
    public class TaxClassHandler : CommerceSyncHandlerBase<TaxClassReadOnly>, ISyncHandler
        , IEventHandlerFor<TaxClassSavedNotification>
        , IEventHandlerFor<TaxClassDeletedNotification>
    {

        public TaxClassHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<TaxClassReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory)
            : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory) { }

        protected override TaxClassReadOnly GetFromService(Guid key)
            => _CommerceApi.GetTaxClass(key);
        protected override void DeleteViaService(TaxClassReadOnly item)
            => _CommerceApi.DeleteTaxClass(item.Id);

        protected override string GetItemName(TaxClassReadOnly item)
            => item.Name;

        protected override IEnumerable<TaxClassReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetTaxClasses(storeId);

        public void Handle(TaxClassSavedNotification notification)
            => CommerceItemSaved(notification.TaxClass);

        public void Handle(TaxClassDeletedNotification notification)
            => CommerceItemDeleted(notification.TaxClass);
    }
}
