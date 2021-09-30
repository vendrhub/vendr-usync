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
    [SyncHandler("vendrTaxClassHandler", "Taxes", "Vendr\\TaxClass", VendrConstants.Priorites.TaxClass,
        Icon = "icon-library", EntityType = VendrConstants.UdiEntityType.TaxClass)]
    public class TaxClassHandler : VendrSyncHandlerBase<TaxClassReadOnly>, ISyncExtendedHandler
    {
        public override string Group => VendrConstants.Group;

        public TaxClassHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<TaxClassReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        {
        }

        protected override void DeleteViaService(TaxClassReadOnly item)
            => _vendrApi.DeleteTaxClass(item.Id);

        protected override IEnumerable<TaxClassReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetTaxClasses(storeId);

        protected override TaxClassReadOnly GetFromService(Guid key)
            => _vendrApi.GetTaxClass(key);

        protected override string GetItemName(TaxClassReadOnly item)
            => item.Name;

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnTaxClassSaved((e) => VendrItemSaved(e.TaxClass));
            EventHub.NotificationEvents.OnTaxClassDeleted((e) => VendrItemDeleted(e.TaxClass));
        }
    }
}
