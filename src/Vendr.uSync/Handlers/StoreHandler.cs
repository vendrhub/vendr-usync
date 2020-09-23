using System;
using System.Collections.Generic;
using System.Linq;

using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

using uSync8.BackOffice.Configuration;
using uSync8.BackOffice.Services;
using uSync8.BackOffice.SyncHandlers;
using uSync8.Core;
using uSync8.Core.Serialization;

using Vendr.Core.Api;
using Vendr.Core.Events;
using Vendr.Core.Models;

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrStoreHandler", "Stores", "vendr\\Stores", VendrConstants.Priorites.Stores,
        Icon = "icon-store", IsTwoPass = true)]
    public class StoreHandler : VendrSyncHandlerBase<StoreReadOnly>, ISyncExtendedHandler, ISyncPostImportHandler
    {
        // puts the handler into a 'vendr' group, so they can be synced seperatly from settings and content.
        public override string Group => VendrConstants.Group;

        public StoreHandler(
            IVendrApi vendrApi,
            IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<StoreReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService)
            : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }

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

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnStoreSaved((e) => VendrItemSaved(e.Store));
            EventHub.NotificationEvents.OnStoreDeleted((e) => VendrItemDeleted(e.Store));
        }
    }
}
