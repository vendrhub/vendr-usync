using System;
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
    [SyncHandler("vendrEmailTemplateHandler", "Email Templates", "Vendr\\EmailTemplate", VendrConstants.Priorites.EmailTemplate,
        Icon = "icon-mailbox", EntityType = VendrConstants.UdiEntityType.EmailTemplate)]
    public class EmailTemplateHandler : VendrSyncHandlerBase<EmailTemplateReadOnly>, ISyncVendrHandler
    {
        public EmailTemplateHandler(IVendrApi vendrApi, ILogger<VendrSyncHandlerBase<EmailTemplateReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(vendrApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(EmailTemplateReadOnly item)
            => _vendrApi.DeleteEmailTemplate(item.Id);

        protected override IEnumerable<EmailTemplateReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetEmailTemplates(storeId);

        protected override EmailTemplateReadOnly GetFromService(Guid key)
            => _vendrApi.GetEmailTemplate(key);

        protected override string GetItemName(EmailTemplateReadOnly item)
            => item.Name;

        public void Handle(EmailTemplateSavedNotification notification)
            => VendrItemSaved(notification.EmailTemplate);

        public void Handle(EmailTemplateDeletedNotification notification)
            => VendrItemDeleted(notification.EmailTemplate);
    }
}
