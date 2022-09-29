using System;
using System.Collections.Generic;
using System.Linq;
using Vendr.Core.Api;
using Vendr.Core.Models;

using uSync.Core.Sync;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

using static Vendr.Umbraco.Constants.Trees.Settings;


namespace Vendr.uSync.SyncManagers
{
    public class OrderSyncManager : ISyncItemManager
    {
        private readonly Dictionary<NodeType, string> _nodeToEntityMapping = new Dictionary<NodeType, string>
        {
            { NodeType.Store, VendrConstants.UdiEntityType.Store },
            { NodeType.OrderStatuses, VendrConstants.UdiEntityType.OrderStatus },
            { NodeType.ShippingMethods, VendrConstants.UdiEntityType.ShippingMethod },
            { NodeType.Countries, VendrConstants.UdiEntityType.Country },
            { NodeType.Currencies, VendrConstants.UdiEntityType.Currency },
            { NodeType.PaymentMethods, VendrConstants.UdiEntityType.PaymentMethod },
            { NodeType.TaxClasses, VendrConstants.UdiEntityType.TaxClass },
            { NodeType.EmailTemplates, VendrConstants.UdiEntityType.EmailTemplate },
            { NodeType.ExportTemplates, VendrConstants.UdiEntityType.ExportTemplate },
            { NodeType.PrintTemplates, VendrConstants.UdiEntityType.PrintTemplate }
        };

        public string[] EntityTypes => _nodeToEntityMapping.Values.ToArray();

        public string[] Trees => new string[] { Alias };

        private readonly IVendrApi _vendrApi;

        public OrderSyncManager(IVendrApi vendrApi)
        {
            _vendrApi = vendrApi;
        }

        /// <summary>
        ///  return the local entity, based on what the user picked from the tree.
        /// </summary>
        /// <remarks>
        ///  the localitem is enough for uSync to start a sync process it tells us
        ///  the Id, Udi & Entity type of an item (and the name for nice UI)
        /// </remarks>
        public SyncLocalItem GetEntity(SyncTreeItem treeItem)
        {
            var entityType = GetEntityTypeFromTree(treeItem);
            if (string.IsNullOrEmpty(entityType)) return null;

            switch (entityType)
            {
                case VendrConstants.UdiEntityType.Store:
                    return GetStoreItem(treeItem.Id);
                default:
                    return GetStoreSubItem(treeItem.Id, treeItem.QueryStrings["storeId"], entityType);
            }

        }

        private SyncLocalItem GetStoreItem(string id)
        {
            // only showing the menu for the store 
            var storeGuid = GetStoreGuid(id);
            if (storeGuid == null) return null;

            // the isVendrStore proved this was a guid.

            var store = _vendrApi.GetStore(storeGuid.Value);
            if (store == null) return null;

            return new SyncLocalItem
            {
                EntityType = VendrConstants.UdiEntityType.Store,
                Id = store.Id.ToString(),
                Name = store.Name,
                Udi = Udi.Create(VendrConstants.UdiEntityType.Store, store.Id)
            };
        }

        private StoreReadOnly GetStoreById(string id)
        {
            var storeId = GetStoreGuid(id);
            if (storeId == null) return null;
            return _vendrApi.GetStore(storeId.Value);
        }

        /// <summary>
        ///  a sub item of the store - we return the 'root' item this type - as we are going to sync it all.
        /// </summary>
        private SyncLocalItem GetStoreSubItem(string id, string storeId, string entityType)
        {
            var store = GetStoreById(storeId);

            return new SyncLocalItem
            {
                EntityType = entityType,
                Id = id,
                Name = $"{store.Name} {entityType}",
                Udi = Udi.Create(entityType, store.Id),
            };
        }

        public IEnumerable<SyncItem> GetItems(SyncItem item)
        {
            // for the store just return ths store item,
            // the depdency checker will do the rest.
            if (item.Udi.EntityType == VendrConstants.UdiEntityType.Store)
                return item.AsEnumerableOfOne();

            // for other items the ID might be the store ID 
            // which acts as a root Udi for that type in the store. 
            if (item.Udi is GuidUdi guidUdi)
            {
                var store = _vendrApi.GetStore(guidUdi.Guid);
                if (store == null) return item.AsEnumerableOfOne();

                // if it was the store, get all the items of that type 

                // there might be a more generic way of doing this ?
                switch (item.Udi.EntityType)
                {
                    case VendrConstants.UdiEntityType.OrderStatus:
                        return _vendrApi.GetOrderStatuses(store.Id)
                            .Select(x => new SyncItem
                            {
                                Name = x.Name,
                                Udi = Udi.Create(VendrConstants.UdiEntityType.OrderStatus, x.Id),
                                Flags = item.Flags,
                            });
                            
                }
            }
            return item.AsEnumerableOfOne();
        }


        


        /// <summary>
        ///  uSync Exporter - supply the info for it to open the picker. 
        /// </summary>
        public SyncEntityInfo GetSyncInfo(string entityType)
        {
            var x = entityType;
            return null;
        }

        /// <summary>
        ///  tells usync what type of sync this is (settings, content, files)
        /// </summary>
        public SyncTreeType GetTreeType(SyncTreeItem treeItem)
        {
            var entityType = GetEntityTypeFromTree(treeItem);
            if (entityType != null) return SyncTreeType.Settings;

            return SyncTreeType.None;
        }

        private Guid? GetStoreGuid(string id)
        {
            if (Guid.TryParse(id, out Guid storeGuid))
                return storeGuid;

            return null;
        }



        private string GetEntityTypeFromTree(SyncTreeItem item)
        {
            if (GetStoreGuid(item.Id) != null) return VendrConstants.UdiEntityType.Store;

            var storeId = item.QueryStrings?["storeId"];
            if (string.IsNullOrWhiteSpace(storeId)) return string.Empty;

            var attempt = item.Id.TryConvertTo<int>();
            if (!attempt.Success) return string.Empty;

            var vendrNodeType = Ids.FirstOrDefault(x => x.Value == attempt.Result).Key;

            if (_nodeToEntityMapping.ContainsKey(vendrNodeType))
                return _nodeToEntityMapping[vendrNodeType];

            return string.Empty;
        }
    }
}
