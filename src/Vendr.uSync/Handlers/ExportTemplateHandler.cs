﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

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
    [SyncHandler("vendrExportTemplateHandler", "Export Templates", "Vendr\\ExportTemplate", VendrConstants.Priorites.ExportTemplate,
        Icon = "icon-sharing-iphone", EntityType = VendrConstants.UdiEntityType.ExportTemplate)]
    public class ExportTemplateHandler : VendrSyncHandlerBase<ExportTemplateReadOnly>, ISyncVendrHandler
    {
        public ExportTemplateHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<ExportTemplateReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(ExportTemplateReadOnly item)
            => _vendrApi.DeleteExportTemplate(item.Id);

        protected override IEnumerable<ExportTemplateReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetExportTemplates(storeId);

        protected override ExportTemplateReadOnly GetFromService(Guid key)
            => _vendrApi.GetExportTemplate(key);

        protected override string GetItemName(ExportTemplateReadOnly item)
            => item.Name;

        public void Handle(ExportTemplateSavedNotification notification)
            => VendrItemSaved(notification.ExportTemplate);

        public void Handle(ExportTemplateDeletedNotification notification)
            => VendrItemDeleted(notification.ExportTemplate);
    }
}
