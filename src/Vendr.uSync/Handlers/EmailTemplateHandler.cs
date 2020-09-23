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
    [SyncHandler("vendrEmailTemplateHandler", "Email Templates", "Vendr\\EmailTemplate", VendrConstants.Priorites.EmailTemplate,
        Icon = "icon-mailbox")]
    public class EmailTemplateHandler : VendrSyncHandlerBase<EmailTemplateReadOnly>, ISyncExtendedHandler
    {
        public override string Group => VendrConstants.Group;

        public EmailTemplateHandler(IVendrApi vendrApi, IProfilingLogger logger, AppCaches appCaches, ISyncSerializer<EmailTemplateReadOnly> serializer, ISyncItemFactory itemFactory, SyncFileService syncFileService) : base(vendrApi, logger, appCaches, serializer, itemFactory, syncFileService)
        {
        }

        protected override void DeleteViaService(EmailTemplateReadOnly item)
            => _vendrApi.DeleteEmailTemplate(item.Id);

        protected override IEnumerable<EmailTemplateReadOnly> GetByStore(Guid storeId)
            => _vendrApi.GetEmailTemplates(storeId);

        protected override EmailTemplateReadOnly GetFromService(Guid key)
            => _vendrApi.GetEmailTemplate(key);

        protected override string GetItemName(EmailTemplateReadOnly item)
            => item.Name;

        protected override void InitializeEvents(HandlerSettings settings)
        {
            EventHub.NotificationEvents.OnEmailTemplateSaved((e) => VendrItemSaved(e.EmailTemplate));
            EventHub.NotificationEvents.OnEmailTemplateDeleted((e) => VendrItemDeleted(e.EmailTemplate));
        }
    }
}
