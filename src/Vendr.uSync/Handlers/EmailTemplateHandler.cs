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
    [SyncHandler("vendrEmailTemplateHandler", "Email Templates", "Vendr\\EmailTemplate", VendrConstants.Priorites.EmailTemplate,
        Icon = "icon-mailbox", EntityType = VendrConstants.UdiEntityType.EmailTemplate)]
    public class EmailTemplateHandler : VendrSyncHandlerBase<EmailTemplateReadOnly>, ISyncVendrHandler
    {
#if NETFRAMEWORK
        public EmailTemplateHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<EmailTemplateReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        { }
#else
        public EmailTemplateHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<EmailTemplateReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }
#endif

        protected override void DeleteViaService(EmailTemplateReadOnly item)
            => _vendrApi.DeleteEmailTemplate(item.Id);

        protected override IEnumerable<EmailTemplateReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetEmailTemplates(storeId);

        protected override EmailTemplateReadOnly GetFromService(Guid key)
            => _vendrApi.GetEmailTemplate(key);

        protected override string GetItemName(EmailTemplateReadOnly item)
            => item.Name;

        public void Handle(IEvent evt)
        {
            switch (evt)
            {
                case EmailTemplateSavedNotification savedNotification:
                    VendrItemSaved(savedNotification.EmailTemplate);
                    break;
                case EmailTemplateDeletedNotification deletedNotification:
                    VendrItemDeleted(deletedNotification.EmailTemplate);
                    break;
            }
        }

    }
}
