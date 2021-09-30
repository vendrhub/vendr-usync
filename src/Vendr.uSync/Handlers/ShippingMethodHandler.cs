using System;
using System.Collections.Generic;

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
    [SyncHandler("vendrShippingMethodHandler", "Shipping Methods", "Vendr\\ShippingMethod", VendrConstants.Priorites.ShippingMethod,
        Icon = "icon-truck", EntityType = VendrConstants.UdiEntityType.ShippingMethod)]
    public class ShippingMethodHandler : VendrSyncHandlerBase<ShippingMethodReadOnly>, ISyncExtendedHandler
    {
        public override string Group => VendrConstants.Group;

        public ShippingMethodHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<ShippingMethodReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService)
            : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }

        protected override IEnumerable<ShippingMethodReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetShippingMethods(storeId);

        protected override void DeleteViaService(ShippingMethodReadOnly item)
            => _vendrApi.DeleteShippingMethod(item.Id);

        protected override ShippingMethodReadOnly GetFromService(Guid key)
            => _vendrApi.GetShippingMethod(key);

        protected override string GetItemName(ShippingMethodReadOnly item)
            => item.Name;

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnShippingMethodSaved((e) => VendrItemSaved(e.ShippingMethod));
            EventHub.NotificationEvents.OnShippingMethodDeleted((e) => VendrItemDeleted(e.ShippingMethod));
        }
    }
}
