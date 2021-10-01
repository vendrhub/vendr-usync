using System;
using System.Collections.Generic;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common.Events;
using Vendr.Core.Events.Notification;

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

using uSync.BackOffice.SyncHandlers;
using uSync.BackOffice.Services;
using uSync.BackOffice.Configuration;
using uSync.Core;
#endif

namespace Vendr.uSync.Handlers
{
    [SyncHandler("vendrPrintTemplateHandler", "Print Templates", "Vendr\\PrintTemplate", VendrConstants.Priorites.PrintTemplate,
        Icon = "icon-print", EntityType = VendrConstants.UdiEntityType.PrintTemplate)]
    public class PrintTemplateHandler : VendrSyncHandlerBase<PrintTemplateReadOnly>, ISyncVendrHandler
        , IEventHandlerFor<PrintTemplateSavedNotification>
        , IEventHandlerFor<PrintTemplateDeletedNotification>
    {

#if NETFRAMEWORK
        public PrintTemplateHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<PrintTemplateReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }
#else
        public PrintTemplateHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<PrintTemplateReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }
#endif

        protected override void DeleteViaService(PrintTemplateReadOnly item)
            => _vendrApi.DeletePrintTemplate(item.Id);

        protected override IEnumerable<PrintTemplateReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetPrintTemplates(storeId);

        protected override PrintTemplateReadOnly GetFromService(Guid key)
            => _vendrApi.GetPrintTemplate(key);

        protected override string GetItemName(PrintTemplateReadOnly item)
            => item.Name;

        public void Handle(IEvent evt)
        {
            switch (evt)
            {
                case PrintTemplateSavedNotification savedNotification:
                    VendrItemSaved(savedNotification.PrintTemplate);
                    break;
                case PrintTemplateDeletedNotification deletedNotification:
                    VendrItemDeleted(deletedNotification.PrintTemplate);
                    break;
            }
        }
    }
}
