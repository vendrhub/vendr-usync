using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Vendr.Common.Events;
using Vendr.Core.Api;
using Vendr.Core.Events.Notification;
using Vendr.Core.Models;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;

using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrPaymentMethodHandler", "Payment Methods", "Vendr\\PaymentMethod", VendrConstants.Priorites.PaymentMethod,
        Icon = "icon-multiple-credit-cards", EntityType = VendrConstants.UdiEntityType.PaymentMethod)]
    public class PaymentMethodHandler : VendrSyncHandlerBase<PaymentMethodReadOnly>, ISyncVendrHandler
        , IEventHandlerFor<PaymentMethodSavedNotification>
        , IEventHandlerFor<PaymentMethodDeletedNotification>
    {

        public PaymentMethodHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<PaymentMethodReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override IEnumerable<PaymentMethodReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetPaymentMethods(storeId);

        protected override void DeleteViaService(PaymentMethodReadOnly item)
            => _vendrApi.DeletePaymentMethod(item.Id);

        protected override PaymentMethodReadOnly GetFromService(Guid key)
            => _vendrApi.GetPaymentMethod(key);

        protected override string GetItemName(PaymentMethodReadOnly item)
            => item.Name;

        public void Handle(PaymentMethodSavedNotification notification)
            => VendrItemSaved(notification.PaymentMethod);

        public void Handle(PaymentMethodDeletedNotification notification)
            => VendrItemDeleted(notification.PaymentMethod);
    }
}
