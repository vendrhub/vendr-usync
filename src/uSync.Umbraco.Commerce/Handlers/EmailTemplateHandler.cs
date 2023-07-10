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
    [SyncHandler("CommerceEmailTemplateHandler", "Email Templates", "Commerce\\EmailTemplate", CommerceConstants.Priorites.EmailTemplate,
        Icon = "icon-mailbox", EntityType = CommerceConstants.UdiEntityType.EmailTemplate)]
    public class EmailTemplateHandler : CommerceSyncHandlerBase<EmailTemplateReadOnly>, ISyncHandler
    {
        public EmailTemplateHandler(ICommerceApi CommerceApi, ILogger<CommerceSyncHandlerBase<EmailTemplateReadOnly>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory) : base(CommerceApi, logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        { }

        protected override void DeleteViaService(EmailTemplateReadOnly item)
            => _CommerceApi.DeleteEmailTemplate(item.Id);

        protected override IEnumerable<EmailTemplateReadOnly> GetByStore(Guid storeId)
            => _CommerceApi.GetEmailTemplates(storeId);

        protected override EmailTemplateReadOnly GetFromService(Guid key)
            => _CommerceApi.GetEmailTemplate(key);

        protected override string GetItemName(EmailTemplateReadOnly item)
            => item.Name;

        public void Handle(EmailTemplateSavedNotification notification)
            => CommerceItemSaved(notification.EmailTemplate);

        public void Handle(EmailTemplateDeletedNotification notification)
            => CommerceItemDeleted(notification.EmailTemplate);
    }
}
