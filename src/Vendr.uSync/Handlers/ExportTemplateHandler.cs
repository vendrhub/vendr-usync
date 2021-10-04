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
    [SyncHandler("vendrExportTemplateHandler", "Export Templates", "Vendr\\ExportTemplate", VendrConstants.Priorites.ExportTemplate,
        Icon = "icon-sharing-iphone", EntityType = VendrConstants.UdiEntityType.ExportTemplate)]
    public class ExportTemplateHandler : VendrSyncHandlerBase<ExportTemplateReadOnly>, ISyncVendrHandler
    {
#if NETFRAMEWORK
        public ExportTemplateHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<ExportTemplateReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }
#else
        public ExportTemplateHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<ExportTemplateReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }
#endif

        protected override void DeleteViaService(ExportTemplateReadOnly item)
            => _vendrApi.DeleteExportTemplate(item.Id);

        protected override IEnumerable<ExportTemplateReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetExportTemplates(storeId);

        protected override ExportTemplateReadOnly GetFromService(Guid key)
            => _vendrApi.GetExportTemplate(key);

        protected override string GetItemName(ExportTemplateReadOnly item)
            => item.Name;

        public void Handle(IEvent evt)
        {
            switch (evt)
            {
                case ExportTemplateSavedNotification savedNotification:
                    VendrItemSaved(savedNotification.ExportTemplate);
                    break;
                case ExportTemplateDeletedNotification deletedNotification:
                    VendrItemDeleted(deletedNotification.ExportTemplate);
                    break;
            }
        }
    }
}
