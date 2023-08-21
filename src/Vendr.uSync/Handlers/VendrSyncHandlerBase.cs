﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common.Events;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;
using uSync.Core;
using uSync.Core.Serialization;
using uSync.BackOffice;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using Microsoft.Extensions.Logging;

namespace Vendr.uSync.Handlers
{
    public abstract class VendrSyncHandlerBase<TObject> :
        SyncHandlerRoot<TObject, TObject>, IEventHandler
        where TObject : EntityBase
    {
        protected IVendrApi _vendrApi;
        public override string Group => VendrConstants.Group;

        public VendrSyncHandlerBase(
            IVendrApi vendrApi,
            ILogger<VendrSyncHandlerBase<TObject>> logger, 
            AppCaches appCaches, 
            IShortStringHelper shortStringHelper, 
            SyncFileService syncFileService, 
            uSyncEventService mutexService, 
            uSyncConfigService uSyncConfig, 
            ISyncItemFactory itemFactory)
            : base(logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
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
        ///  On the vendr base class means, it can be applied to any of the handler configs.
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

        protected virtual void VendrItemSaved(TObject item)
        {
            if (!ShouldProcessEvent()) return;

            Export(item, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
        }

        protected virtual void VendrItemDeleted(TObject item)
        {
            if (!ShouldProcessEvent()) return;

            ExportDeletedItem(item, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
        }

        /// <summary>
        ///  checks to see if we should process the vendr events/notifications
        /// </summary>
        private new bool ShouldProcessEvent()
        {
            if (_mutexService.IsPaused) return false;
            if (!DefaultConfig.Enabled) return false;
            return true;
        }

        /// <summary>
        ///  make the handling of vendr events a bit more generic, so we can clean up the handler code a bit. 
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
