
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

using uSync8.BackOffice;
using uSync8.BackOffice.Configuration;
using uSync8.BackOffice.Services;
using uSync8.BackOffice.SyncHandlers;
using uSync8.Core;
using uSync8.Core.Serialization;

using Vendr.Core.Api;
using Vendr.Core.Models;

namespace Vendr.uSync.Handlers
{
    public abstract class VendrSyncHandlerBase<TObject> :
        SyncHandlerRoot<TObject, TObject> 
        where TObject : EntityBase
    {
        protected IVendrApi _vendrApi;

        public VendrSyncHandlerBase(
            IVendrApi vendrApi,
            IProfilingLogger logger,
            AppCaches appCaches,
            ISyncSerializer<TObject> serializer,
            ISyncItemFactory itemFactory,
            SyncFileService syncFileService)
            : base(logger, appCaches, serializer, itemFactory, syncFileService)
        {
            _vendrApi = vendrApi;
        }

        public virtual IEnumerable<uSyncAction> ProcessPostImport(string folder, IEnumerable<uSyncAction> actions, HandlerSettings config)
        {
            if (actions == null || !actions.Any()) return null;

            foreach (var action in actions)
            {
                var result = Import(action.FileName, config, SerializerFlags.None);
                if (result.Success)
                {
                    ImportSecondPass(action.FileName, result.Item, config, null);
                }
            }

            return actions;
        }

        /// <summary>
        ///  Handles the deleting of items in Umbraco but not the sync.
        /// </summary>
        /// <remarks>
        ///  this isn't always used, its only when a user explicity asks for 
        ///  the folder to be cleaned - are things deleted this way.
        ///  TODO: this ideally should be implimented
        /// </remarks>
        protected override IEnumerable<uSyncAction> DeleteMissingItems(TObject parent, IEnumerable<Guid> keysToKeep, bool reportOnly)
                => Enumerable.Empty<uSyncAction>();


        protected override TObject GetFromService(int id)
            => default;

        protected override TObject GetFromService(string alias)
            => default;

        protected override TObject GetFromService(TObject item)
            => item;

        protected override Guid GetItemKey(TObject item)
            => item.Id;

        protected override IEnumerable<TObject> GetFolders(TObject parent)
            => Enumerable.Empty<TObject>();

        protected override string GetItemPath(TObject item, bool useGuid, bool isFlat)
            => useGuid ? item.Id.ToString() : GetItemName(item).ToSafeFileName();

        protected virtual void VendrItemSaved(TObject item)
        {
            if (uSync8BackOffice.eventsPaused) return;
            Export(item, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
        }

        protected virtual void VendrItemDeleted(TObject item)
        {
            if (uSync8BackOffice.eventsPaused) return;
            ExportDeletedItem(item, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);

        }

    }
}
