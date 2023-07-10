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
    [SyncHandler("CommercePrintTemplateHandler", "Print Templates", "Commerce\\PrintTemplate", CommerceConstants.Priorites.PrintTemplate,
        Icon = "icon-print", EntityType = CommerceConstants.UdiEntityType.PrintTemplate)]
    public class PrintTemplateHandler : CommerceSyncHandlerBase<PrintTemplateReadOnly>, ISyncHandler
        , IEventHandlerFor<PrintTemplateSavedNotification>
        , IEventHandlerFor<PrintTemplateDeletedNotification>
    {

        public PrintTemplateHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<PrintTemplateReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(PrintTemplateReadOnly item)
            => _CommerceApi.DeletePrintTemplate(item.Id);

        protected override IEnumerable<PrintTemplateReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetPrintTemplates(storeId);

        protected override PrintTemplateReadOnly GetFromService(Guid key)
            => _CommerceApi.GetPrintTemplate(key);

        protected override string GetItemName(PrintTemplateReadOnly item)
            => item.Name;

        public void Handle(PrintTemplateSavedNotification notification)
            => CommerceItemSaved(notification.PrintTemplate);

        public void Handle(PrintTemplateDeletedNotification notification)
            => CommerceItemDeleted(notification.PrintTemplate);
    }
}
