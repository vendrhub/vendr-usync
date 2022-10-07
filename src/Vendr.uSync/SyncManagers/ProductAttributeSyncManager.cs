using System;
using System.Collections.Generic;
using System.Linq;

using Umbraco.Cms.Core;
using Umbraco.Extensions;

using uSync.Core.Sync;

using Vendr.Core.Api;
using Vendr.Core.Models;

using static Vendr.Umbraco.Constants.Trees.Stores;

namespace Vendr.uSync.SyncManagers;
public class ProductAttributeSyncManager : ISyncItemManager
{
    private readonly IVendrApi _vendrApi;

    public ProductAttributeSyncManager(IVendrApi vendrApi)
    {
        _vendrApi = vendrApi;
    }


    /// <summary>
    ///  mappings helps us get to and from a UDI to an Vendr NodeType
    /// </summary>
    private readonly Dictionary<string, string> _mappings = new Dictionary<string, string>
    {
        { nameof(NodeType.ProductAttributes), VendrConstants.UdiEntityType.ProductAttribute }
    };


    /// <summary>
    ///  what entity types do we support ? 
    /// </summary>
    public string[] EntityTypes => _mappings.Values.ToArray();

    /// <summary>
    ///  what Umbraco tree do the items live on
    /// </summary>
    public string[] Trees => new[] { Vendr.Umbraco.Constants.Trees.Stores.Alias };

    /// <summary>
    ///  return an entity based on the info we get from the tree.
    /// </summary>
    public SyncLocalItem GetEntity(SyncTreeItem treeItem)
    {
        var entityType = GetEntityTypeFromTree(treeItem);
        if (string.IsNullOrEmpty(entityType)) return null;
        
        return GetStoreSubItem(treeItem.Id, treeItem.QueryStrings["storeId"], entityType); 
    }

    private SyncLocalItem GetStoreSubItem(string id, string storeId, string entityType)
    {
        var store = GetStoreById(storeId);

        return new SyncLocalItem
        {
            EntityType = entityType,
            Id = id,
            Name = $"{store.Name} {entityType}",
            Udi = Udi.Create(entityType, store.Id)
        };
    }

    /// <summary>
    ///  publisher uses this as the starting point, the root items we will sync.
    /// </summary>
    public IEnumerable<SyncItem> GetItems(SyncItem item)
    {
        if (item.Udi is GuidUdi udi)
        {
            var attributes = _vendrApi.GetProductAttributes(udi.Guid);

            return attributes.Select(x => new SyncItem
            {
                Name = x.Name,
                Udi = Udi.Create(VendrConstants.UdiEntityType.ProductAttribute, x.Id),
                Flags = item.Flags
            });
        }

        return item.AsEnumerableOfOne();
        
    }

    /// <summary>
    ///  uSync.Exporter - if you return the entity info, then exporter 
    ///  can use this to open a picker on its dashboard.
    /// </summary>
    /// <remarks>
    ///  if this is null, then the item doesn't appear pickable in uSync.Exporter
    /// </remarks>
    public SyncEntityInfo GetSyncInfo(string entityType)
        => null;

    /// <summary>
    ///  tells uSync.Publisher what type of 'sync' we are doing. 
    /// </summary>
    /// <remarks>
    ///  in general there are content, settings and file syncs, and this controls
    ///  what options a user sees, almost always you will want settings. 
    /// </remarks>
    public SyncTreeType GetTreeType(SyncTreeItem treeItem)
        => SyncTreeType.Settings;


    /// <summary>
    ///  helper function to get the UdiEntityType based on the values 
    ///  in the Umbraco tree query. 
    /// </summary>
    private string GetEntityTypeFromTree(SyncTreeItem item)
    {
        var nodeType = item.QueryStrings?["nodeType"];
        if (string.IsNullOrEmpty(nodeType)) return null;

        if (_mappings.ContainsKey(nodeType))
            return _mappings[nodeType];

        return null;

    }

    private Guid? GetStoreGuid(string id)
    {
        if (Guid.TryParse(id, out Guid storeGuid))
            return storeGuid;

        return null;
    }

    private StoreReadOnly GetStoreById(string id)
    {
        var storeId = GetStoreGuid(id);
        if (storeId == null) return null;
        return _vendrApi.GetStore(storeId.Value);
    }
}
