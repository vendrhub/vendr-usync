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
    /// <summary>
    ///  Handler for Country entries in Commerce
    /// </summary>
    /// <remarks>
    ///  PostImportHandler means the import is ran again at the end, because it depends on payment & shipping
    ///  which have to run after country as they depend on them.
    /// </remarks>
    [SyncHandler("CommerceCountryHandler", "Countries", "Commerce\\Country", CommerceConstants.Priorites.Country,
        Icon = "icon-globe", IsTwoPass = true, EntityType = CommerceConstants.UdiEntityType.Country)]
    public class CountryHandler : CommerceSyncHandlerBase<CountryReadOnly>, ISyncPostImportHandler, ISyncHandler
    {
        public CountryHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<CountryReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(CountryReadOnly item)
            => _CommerceApi.DeleteCountry(item.Id);

        protected override IEnumerable<CountryReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetCountries(storeId);

        protected override CountryReadOnly GetFromService(Guid key)
            => _CommerceApi.GetCountry(key);

        protected override string GetItemName(CountryReadOnly item)
            => item.Name;

        public void Handle(CountrySavedNotification notification)
            => CommerceItemSaved(notification.Country);

        public void Handle(CountryDeletedNotification notification)
            => CommerceItemDeleted(notification.Country);
    }
}
