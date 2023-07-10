using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [SyncHandler("CommerceStoreHandler", "Stores", "Commerce\\Stores", CommerceConstants.Priorites.Stores,
        Icon = "icon-store", IsTwoPass = true, EntityType = CommerceConstants.UdiEntityType.Store)]
    public class StoreHandler : CommerceSyncHandlerBase<StoreReadOnly>, ISyncHandler, ISyncPostImportHandler
        , IEventHandlerFor<StoreSavedNotification>
        , IEventHandlerFor<StoreDeletedNotification>
    {
        public StoreHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<StoreReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory)
            : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory) { }

        /// <summary>
        ///  Delete a store 
        /// </summary>
        /// <remarks>
        ///  This is called when an 'empty' file is found with a delete instruction in it. 
        ///  These files are created when a user deletes a store
        /// </remarks>
        protected override void DeleteViaService(StoreReadOnly item)
            => _CommerceApi.DeleteStore(item.Id);

        /// <summary>
        ///  get the child items, for a given store.
        /// </summary>
        /// <remarks>
        ///  if we are at the root, then the store will be null, and we should
        ///  return all items at the top level.
        /// </remarks>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected override IEnumerable<StoreReadOnly> GetChildItems(StoreReadOnly parent)
        {
            if (parent == null)
            {
                return _CommerceApi.GetStores();
            }

            return Enumerable.Empty<StoreReadOnly>();
        }

        /// <summary>
        ///  Get store by key
        /// </summary>
        protected override StoreReadOnly GetFromService(Guid key)
            => _CommerceApi.GetStore(key);

        /// <summary>
        ///  get store by store alias
        /// </summary>
        protected override StoreReadOnly GetFromService(string alias)
            => _CommerceApi.GetStore(alias);

        protected override string GetItemName(StoreReadOnly item)
            => item.Name;

        public void Handle(StoreSavedNotification notification)
            => CommerceItemSaved(notification.Store);

        public void Handle(StoreDeletedNotification notification)
            => CommerceItemDeleted(notification.Store);
    }
}
