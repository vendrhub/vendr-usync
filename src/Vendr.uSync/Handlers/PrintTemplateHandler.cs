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
    [SyncHandler("vendrPrintTemplateHandler", "Print Templates", "Vendr\\PrintTemplate", VendrConstants.Priorites.PrintTemplate,
        Icon = "icon-print", EntityType = VendrConstants.UdiEntityType.PrintTemplate)]
    public class PrintTemplateHandler : VendrSyncHandlerBase<PrintTemplateReadOnly>, ISyncExtendedHandler
    {
        public override string Group => VendrConstants.Group;

        public PrintTemplateHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<PrintTemplateReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        {
        }

        protected override void DeleteViaService(PrintTemplateReadOnly item)
            => _vendrApi.DeletePrintTemplate(item.Id);

        protected override IEnumerable<PrintTemplateReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetPrintTemplates(storeId);

        protected override PrintTemplateReadOnly GetFromService(Guid key)
            => _vendrApi.GetPrintTemplate(key);

        protected override string GetItemName(PrintTemplateReadOnly item)
            => item.Name;

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnPrintTemplateSaved((e) => VendrItemSaved(e.PrintTemplate));
            EventHub.NotificationEvents.OnPrintTemplateDeleted((e) => VendrItemDeleted(e.PrintTemplate));
        }
    }
}
