using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common.Events;
using Vendr.Core.Events.Notification;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;

using uSync.BackOffice.SyncHandlers;
using uSync.BackOffice.Services;
using uSync.BackOffice.Configuration;
using uSync.Core;

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrPrintTemplateHandler", "Print Templates", "Vendr\\PrintTemplate", VendrConstants.Priorites.PrintTemplate,
        Icon = "icon-print", EntityType = VendrConstants.UdiEntityType.PrintTemplate)]
    public class PrintTemplateHandler : VendrSyncHandlerBase<PrintTemplateReadOnly>, ISyncVendrHandler
        , IEventHandlerFor<PrintTemplateSavedNotification>
        , IEventHandlerFor<PrintTemplateDeletedNotification>
    {
        public PrintTemplateHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<PrintTemplateReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(PrintTemplateReadOnly item)
            => _vendrApi.DeletePrintTemplate(item.Id);

        protected override IEnumerable<PrintTemplateReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetPrintTemplates(storeId);

        protected override PrintTemplateReadOnly GetFromService(Guid key)
            => _vendrApi.GetPrintTemplate(key);

        protected override string GetItemName(PrintTemplateReadOnly item)
            => item.Name;

        public void Handle(PrintTemplateSavedNotification notification)
            => VendrItemSaved(notification.PrintTemplate);

        public void Handle(PrintTemplateDeletedNotification notification)
            => VendrItemDeleted(notification.PrintTemplate);
    }
}
