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
    [SyncHandler("vendrPaymentMethodHandler", "Payment Methods", "Vendr\\PaymentMethod", VendrConstants.Priorites.PaymentMethod,
        Icon = "icon-multiple-credit-cards", EntityType = VendrConstants.UdiEntityType.PaymentMethod)]
    public class PaymentMethodHandler : VendrSyncHandlerBase<PaymentMethodReadOnly>, ISyncExtendedHandler
    {
        public override string Group => VendrConstants.Group;

        public PaymentMethodHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<PaymentMethodReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService)
            : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }

        protected override IEnumerable<PaymentMethodReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetPaymentMethods(storeId);

        protected override void DeleteViaService(PaymentMethodReadOnly item)
            => _vendrApi.DeletePaymentMethod(item.Id);

        protected override PaymentMethodReadOnly GetFromService(Guid key)
            => _vendrApi.GetPaymentMethod(key);

        protected override string GetItemName(PaymentMethodReadOnly item)
            => item.Name;

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnPaymentMethodSaved((e) => VendrItemSaved(e.PaymentMethod));
            EventHub.NotificationEvents.OnPaymentMethodDeleted((e) => VendrItemDeleted(e.PaymentMethod));
        }
    }
}
