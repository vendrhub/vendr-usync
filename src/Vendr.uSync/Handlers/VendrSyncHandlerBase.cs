
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

using uSync8.BackOffice;
using uSync8.BackOffice.Configuration;
using uSync8.BackOffice.Services;
using uSync8.BackOffice.SyncHandlers;
using uSync8.Core;
using uSync8.Core.Extensions;
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

        /// <summary>
        ///  get the item by store 
        /// </summary>
        protected virtual IEnumerable<TObject> GetByStore(Guid storeId)
            => Enumerable.Empty<TObject>();

        protected override IEnumerable<TObject> GetChildItems(TObject parent)
        {
            if (parent != null) return Enumerable.Empty<TObject>();

            var items = new List<TObject>();
            foreach (var store in _vendrApi.GetStores())
            {
                items.AddRange(GetByStore(store.Id));
            }
            return items;
        }

        public virtual IEnumerable<uSyncAction> ProcessPostImport(string folder, IEnumerable<uSyncAction> actions, HandlerSettings config)
        {
            if (actions == null || !actions.Any()) return null;

            var postActions = new List<uSyncAction>();

            foreach (var action in actions)
            {
                var result = Import(action.FileName, config, SerializerFlags.None);
                if (result.Success)
                {
                    var attempt = ImportSecondPass(action.FileName, result.Item, config, null);
                    // postActions.Add();
                }
            }

            return postActions;
        }

        /// <summary>
        ///  if there is a 'OneWay' setting in the config, then we will only import something
        ///  if it doesn't already exist. 
        /// </summary>
        /// <remarks>
        ///  On the vendr base class means, it can be applied to any of the handler configs.
        /// </remarks>
        protected override bool ShouldImport(XElement node, HandlerSettings config)
        {
            if (config.Settings.ContainsKey("OneWay") && config.Settings["OneWay"].InvariantEquals("true"))
            {
                // only import if it doesn't already exist.
                var item = GetFromService(node.GetKey());
                return item == null;
            }

            return false;
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
            => default(TObject);

        protected override TObject GetFromService(string alias)
            => default(TObject);

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
