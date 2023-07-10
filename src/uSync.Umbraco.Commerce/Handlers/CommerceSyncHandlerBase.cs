using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using uSync.BackOffice;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;
using uSync.Core.Serialization;

namespace uSync.Umbraco.Commerce.Handlers
{
    public abstract class CommerceSyncHandlerBase<TObject> :
        SyncHandlerRoot<TObject, TObject>, IEventHandler
        where TObject : EntityBase
    {
        protected ICommerceApi _CommerceApi;
        public override string Group => CommerceConstants.Group;

        public CommerceSyncHandlerBase(
            ICommerceApi CommerceApi,
            ILogger<CommerceSyncHandlerBase<TObject>> logger,
            AppCaches appCaches,
            IShortStringHelper shortStringHelper,
            SyncFileService syncFileService,
            uSyncEventService mutexService,
            uSyncConfigService uSyncConfig,
            ISyncItemFactory itemFactory)
            : base(logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        {
            _CommerceApi = CommerceApi;
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
            foreach (var store in _CommerceApi.GetStores())
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
                var results = Import(action.FileName, config, SerializerFlags.None);
                foreach (var result in results)
                {
                    if (result.Success)
                    {
                        var attempt = ImportSecondPass(result, config, new uSyncImportOptions());
                        // postActions.Add();
                    }
                }
            }

            return postActions;
        }

        /// <summary>
        ///  if there is a 'OneWay' (or CreateOnly) setting in the config, then we will only import something
        ///  if it doesn't already exist. 
        /// </summary>
        /// <remarks>
        ///  On the Commerce base class means, it can be applied to any of the handler configs.
        /// </remarks>
        protected override bool ShouldImport(XElement node, HandlerSettings config)
        {
            if (config.GetSetting("OneWay", false) || config.GetSetting("CreateOnly", false))
            {
                // only import if it doesn't already exist.
                var item = GetFromService(node.GetKey());
                return item == null;
            }

            return base.ShouldImport(node, config);
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

        protected override TObject GetFromService(TObject item)
            => item;

        protected override IEnumerable<TObject> GetFolders(TObject parent)
            => Enumerable.Empty<TObject>();

        protected virtual void CommerceItemSaved(TObject item)
        {
            if (!ShouldProcessEvent()) return;

            Export(item, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
        }

        protected virtual void CommerceItemDeleted(TObject item)
        {
            if (!ShouldProcessEvent()) return;

            ExportDeletedItem(item, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
        }

        /// <summary>
        ///  checks to see if we should process the Commerce events/notifications
        /// </summary>
        private new bool ShouldProcessEvent()
        {
            if (_mutexService.IsPaused) return false;
            if (!DefaultConfig.Enabled) return false;
            return true;
        }

        /// <summary>
        ///  make the handling of Commerce events a bit more generic, so we can clean up the handler code a bit. 
        /// </summary>
        void IEventHandler.Handle(IEvent evt)
        {
            var eventType = evt.GetType();
            if (typeof(INotificationEvent).IsAssignableFrom(eventType))
            {
                var handlerType = typeof(IEventHandlerFor<>).MakeGenericType(eventType);
                if (handlerType.IsAssignableFrom(GetType()))
                {
                    var handleMethod = handlerType.GetMethod("Handle", new[] { eventType });
                    handleMethod.Invoke(this, new[] { evt });
                }
            }
        }
    }
}
