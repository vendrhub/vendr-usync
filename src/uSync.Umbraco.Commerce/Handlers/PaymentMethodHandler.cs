using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Events.Notification;
using Umbraco.Commerce.Core.Models;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

namespace uSync.Umbraco.Commerce.Handlers
{
    [SyncHandler("CommercePaymentMethodHandler", "Payment Methods", "Commerce\\PaymentMethod", CommerceConstants.Priorites.PaymentMethod,
        Icon = "icon-multiple-credit-cards", EntityType = CommerceConstants.UdiEntityType.PaymentMethod)]
    public class PaymentMethodHandler : CommerceSyncHandlerBase<PaymentMethodReadOnly>, ISyncHandler
        , IEventHandlerFor<PaymentMethodSavedNotification>
        , IEventHandlerFor<PaymentMethodDeletedNotification>
    {

        public PaymentMethodHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<PaymentMethodReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override IEnumerable<PaymentMethodReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetPaymentMethods(storeId);

        protected override void DeleteViaService(PaymentMethodReadOnly item)
            => _CommerceApi.DeletePaymentMethod(item.Id);

        protected override PaymentMethodReadOnly GetFromService(Guid key)
            => _CommerceApi.GetPaymentMethod(key);

        protected override string GetItemName(PaymentMethodReadOnly item)
            => item.Name;

        public void Handle(PaymentMethodSavedNotification notification)
            => CommerceItemSaved(notification.PaymentMethod);

        public void Handle(PaymentMethodDeletedNotification notification)
            => CommerceItemDeleted(notification.PaymentMethod);
    }
}
