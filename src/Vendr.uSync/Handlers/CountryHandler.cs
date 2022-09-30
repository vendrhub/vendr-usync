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
    /// <summary>
    ///  Handler for Country entries in vendr
    /// </summary>
    /// <remarks>
    ///  PostImportHandler means the import is ran again at the end, because it depends on payment & shipping
    ///  which have to run after country as they depend on them.
    /// </remarks>
    [SyncHandler("vendrCountryHandler", "Countries", "Vendr\\Country", VendrConstants.Priorites.Country,
        Icon = "icon-globe", IsTwoPass = true, EntityType = VendrConstants.UdiEntityType.Country)]
    public class CountryHandler : VendrSyncHandlerBase<CountryReadOnly>, ISyncPostImportHandler, ISyncVendrHandler
    {
        public CountryHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<CountryReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(CountryReadOnly item)
            => _vendrApi.DeleteCountry(item.Id);

        protected override IEnumerable<CountryReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetCountries(storeId);

        protected override CountryReadOnly GetFromService(Guid key)
            => _vendrApi.GetCountry(key);

        protected override string GetItemName(CountryReadOnly item)
            => item.Name;

        public void Handle(CountrySavedNotification notification)
            => VendrItemSaved(notification.Country);

        public void Handle(CountryDeletedNotification notification)
            => VendrItemDeleted(notification.Country);
    }
}
