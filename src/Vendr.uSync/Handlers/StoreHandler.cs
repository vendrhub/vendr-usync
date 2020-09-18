using System;
using System.Collections.Generic;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using uSync8.BackOffice;
using uSync8.BackOffice.Configuration;
using uSync8.BackOffice.Services;
using uSync8.BackOffice.SyncHandlers;
using uSync8.Core;
using uSync8.Core.Serialization;
using Vendr.Core.Models;
using Vendr.Core.Services;

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrStoreHandler", "Store", "Stores", VendrConstants.Priorites.Stores, Icon = "icon-store")]
    public class StoreHandler : SyncHandlerRoot<StoreReadOnly, IStoreService>, ISyncHandler, ISyncExtendedHandler
    {
        public StoreHandler(IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<StoreReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) 
            : base(logger, appCaches, serializer, itemFactory, syncFileService)
        { }

        protected override IEnumerable<uSyncAction> DeleteMissingItems(StoreReadOnly parent, IEnumerable<Guid> keysToKeep, bool reportOnly)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteViaService(StoreReadOnly item)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IStoreService> GetChildItems(IStoreService parent)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IStoreService> GetFolders(IStoreService parent)
        {
            throw new NotImplementedException();
        }

        protected override StoreReadOnly GetFromService(int id)
            => null;

        protected override StoreReadOnly GetFromService(Guid key)
        {
            throw new NotImplementedException();
        }

        protected override StoreReadOnly GetFromService(string alias)
        {
            throw new NotImplementedException();
        }

        protected override StoreReadOnly GetFromService(IStoreService item)
        {
            throw new NotImplementedException();
        }

        protected override Guid GetItemKey(StoreReadOnly item)
        {
            throw new NotImplementedException();
        }

        protected override string GetItemName(StoreReadOnly item)
        {
            throw new NotImplementedException();
        }

        protected override string GetItemPath(StoreReadOnly item, bool useGuid, bool isFlat)
        {
            throw new NotImplementedException();
        }

        protected override void InitializeEvents(HandlerSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
