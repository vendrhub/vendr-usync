using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;

using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

using Vendr.Common.Events;
using Vendr.Core.Api;
using Vendr.Core.Events.Notification;
using Vendr.Core.Models;

namespace Vendr.uSync.Handlers;

[SyncHandler("vendrProductAttributesHandler", "Product Attributes", "Vendr\\ProductAttrivutes", VendrConstants.Priorites.ProductAttributes,
       Icon = "icon-multiple-credit-cards", EntityType = VendrConstants.UdiEntityType.ProductAttribute)]
public class ProductAttributesHandler : VendrSyncHandlerBase<ProductAttributeReadOnly>, ISyncVendrHandler
    , IEventHandlerFor<ProductAttributeSavedNotification>
    , IEventHandlerFor<ProductAttributeDeletedNotification>

{
    public ProductAttributesHandler(
        IVendrApi vendrApi,
        ILogger<VendrSyncHandlerBase<ProductAttributeReadOnly>> logger,
        AppCaches appCaches,
        IShortStringHelper shortStringHelper,
        SyncFileService syncFileService,
        uSyncEventService mutexService,
        uSyncConfigService uSyncConfig,
        ISyncItemFactory itemFactory) 
        : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
    { }

    protected override IEnumerable<ProductAttributeReadOnly> GetByStore(Guid storeId)
        => _vendrApi.GetProductAttributes(storeId);

    protected override void DeleteViaService(ProductAttributeReadOnly item)
        => _vendrApi.DeleteProductAttribute(item.Id);

    protected override ProductAttributeReadOnly GetFromService(Guid key)
        => _vendrApi.GetProductAttribute(key);

    protected override string GetItemName(ProductAttributeReadOnly item)
        => item.Name;

    public void Handle(ProductAttributeSavedNotification notification)
        => VendrItemSaved(notification.ProductAttribute);

    public void Handle(ProductAttributeDeletedNotification notification)
        => VendrItemDeleted(notification.ProductAttribute);
}
