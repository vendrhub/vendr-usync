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
    [SyncHandler("vendrCurrencyHandler", "Currencies", "vendr\\Currency", VendrConstants.Priorites.Currency,
        Icon = "icon-coins-dollar-alt")]
    public class CurrencyHandler : VendrSyncHandlerBase<CurrencyReadOnly>, ISyncExtendedHandler
    {
        public override string Group => VendrConstants.Group;

        public CurrencyHandler(
            IVendrApi vendrApi,
            IProfilingLogger logger,
            AppCaches appCaches,
            ISyncSerializer<CurrencyReadOnly> serializer,
            ISyncItemFactory itemFactory,
            SyncFileService syncFileService)
            : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }


        protected override void DeleteViaService(CurrencyReadOnly item)
            => _vendrApi.DeleteCurrency(item.Id);

        protected override IEnumerable<CurrencyReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetCurrencies(storeId);

        protected override CurrencyReadOnly GetFromService(Guid key)
            => _vendrApi.GetCurrency(key);

        protected override string GetItemName(CurrencyReadOnly item)
            => item.Name;

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnCurrencySaved((e) => VendrItemSaved(e.Currency));
            EventHub.NotificationEvents.OnCurrencyDeleted((e) => VendrItemDeleted(e.Currency));
        }
    }
}
