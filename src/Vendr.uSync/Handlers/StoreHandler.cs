﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

using Vendr.Core.Api;
using Vendr.Core.Events.Notification;
using Vendr.Core.Models;
using Vendr.Common.Events;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;

using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrStoreHandler", "Stores", "Vendr\\Stores", VendrConstants.Priorites.Stores,
        Icon = "icon-store", IsTwoPass = true, EntityType = VendrConstants.UdiEntityType.Store)]
    public class StoreHandler : VendrSyncHandlerBase<StoreReadOnly>, ISyncVendrHandler, ISyncPostImportHandler
        , IEventHandlerFor<StoreSavedNotification>
        , IEventHandlerFor<StoreDeletedNotification>
    {
        public StoreHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<StoreReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) 
            : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory) { }

        /// <summary>
        ///  Delete a store 
        /// </summary>
        /// <remarks>
        ///  This is called when an 'empty' file is found with a delete instruction in it. 
        ///  These files are created when a user deletes a store
        /// </remarks>
        protected override void DeleteViaService(StoreReadOnly item)
            => _vendrApi.DeleteStore(item.Id);

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
                return _vendrApi.GetStores();
            }

            return Enumerable.Empty<StoreReadOnly>();
        }

        /// <summary>
        ///  Get store by key
        /// </summary>
        protected override StoreReadOnly GetFromService(Guid key)
            => _vendrApi.GetStore(key);

        /// <summary>
        ///  get store by store alias
        /// </summary>
        protected override StoreReadOnly GetFromService(string alias)
            => _vendrApi.GetStore(alias);

        protected override string GetItemName(StoreReadOnly item)
            => item.Name;

        public void Handle(StoreSavedNotification notification)
            => VendrItemSaved(notification.Store);

        public void Handle(StoreDeletedNotification notification)
            => VendrItemDeleted(notification.Store);
    }
}
