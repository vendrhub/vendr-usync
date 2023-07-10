using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Strings;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Events.Notification;
using Umbraco.Commerce.Core.Models;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;

namespace uSync.Umbraco.Commerce.Handlers
{
    [SyncHandler("CommerceExportTemplateHandler", "Export Templates", "Commerce\\ExportTemplate", CommerceConstants.Priorites.ExportTemplate,
        Icon = "icon-sharing-iphone", EntityType = CommerceConstants.UdiEntityType.ExportTemplate)]
    public class ExportTemplateHandler : CommerceSyncHandlerBase<ExportTemplateReadOnly>, ISyncHandler
    {
        public ExportTemplateHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<ExportTemplateReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(ExportTemplateReadOnly item)
            => _CommerceApi.DeleteExportTemplate(item.Id);

        protected override IEnumerable<ExportTemplateReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetExportTemplates(storeId);

        protected override ExportTemplateReadOnly GetFromService(Guid key)
            => _CommerceApi.GetExportTemplate(key);

        protected override string GetItemName(ExportTemplateReadOnly item)
            => item.Name;

        public void Handle(ExportTemplateSavedNotification notification)
            => CommerceItemSaved(notification.ExportTemplate);

        public void Handle(ExportTemplateDeletedNotification notification)
            => CommerceItemDeleted(notification.ExportTemplate);
    }
}
