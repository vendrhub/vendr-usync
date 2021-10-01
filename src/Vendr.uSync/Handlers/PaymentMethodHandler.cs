using System;
using System.Collections.Generic;

using Vendr.Common.Events;
using Vendr.Core.Api;
using Vendr.Core.Events.Notification;
using Vendr.Core.Models;

#if NETFRAMEWORK
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

using uSync8.BackOffice.Services;
using uSync8.BackOffice.SyncHandlers;
using uSync8.Core;
using uSync8.Core.Serialization;
#else 
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;

using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;
#endif

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrPaymentMethodHandler", "Payment Methods", "Vendr\\PaymentMethod", VendrConstants.Priorites.PaymentMethod,
        Icon = "icon-multiple-credit-cards", EntityType = VendrConstants.UdiEntityType.PaymentMethod)]
    public class PaymentMethodHandler : VendrSyncHandlerBase<PaymentMethodReadOnly>, ISyncVendrHandler
        , IEventHandlerFor<PaymentMethodSavedNotification>
        , IEventHandlerFor<PaymentMethodDeletedNotification>
    {

#if NETFRAMEWORK
        public PaymentMethodHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<PaymentMethodReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService)
            : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }
#else
        public PaymentMethodHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<PaymentMethodReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }
#endif

        protected override IEnumerable<PaymentMethodReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetPaymentMethods(storeId);

        protected override void DeleteViaService(PaymentMethodReadOnly item)
            => _vendrApi.DeletePaymentMethod(item.Id);

        protected override PaymentMethodReadOnly GetFromService(Guid key)
            => _vendrApi.GetPaymentMethod(key);

        protected override string GetItemName(PaymentMethodReadOnly item)
            => item.Name;

        public void Handle(IEvent evt)
        {
            switch (evt)
            {
                case PaymentMethodSavedNotification savedNotification:
                    VendrItemSaved(savedNotification.PaymentMethod);
                    break;
                case PaymentMethodDeletedNotification deletedNotification:
                    VendrItemDeleted(deletedNotification.PaymentMethod);
                    break;
            }
        }
    }
}
