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
    [SyncHandler("vendrExportTemplateHandler", "Export Templates", "Vendr\\ExportTemplate", VendrConstants.Priorites.ExportTemplate,
        Icon = "icon-sharing-iphone")]
    public class ExportTemplateHandler : VendrSyncHandlerBase<ExportTemplateReadOnly>, ISyncExtendedHandler
    {
        public override string Group => VendrConstants.Group;

        public ExportTemplateHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<ExportTemplateReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        {
        }

        protected override void DeleteViaService(ExportTemplateReadOnly item)
            => _vendrApi.DeleteExportTemplate(item.Id);

        protected override IEnumerable<ExportTemplateReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetExportTemplates(storeId);

        protected override ExportTemplateReadOnly GetFromService(Guid key)
            => _vendrApi.GetExportTemplate(key);

        protected override string GetItemName(ExportTemplateReadOnly item)
            => item.Name;

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnExportTemplateSaved((e) => VendrItemSaved(e.ExportTemplate));
            EventHub.NotificationEvents.OnExportTemplateDeleted((e) => VendrItemDeleted(e.ExportTemplate));
        }
    }
}
