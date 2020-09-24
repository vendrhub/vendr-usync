using System;
using System.Xml.Linq;

using Umbraco.Core.Logging;

using uSync8.Core;
using uSync8.Core.Extensions;
using uSync8.Core.Models;
using uSync8.Core.Serialization;

using Vendr.Core;
using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.uSync.Extensions;

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("BAEB7691-9AC2-4F42-92DA-2F8CD42B66DE", "Email Template Serializer", VendrConstants.Serialization.EmailTemplate)]

    public class EmailTemplateSerializer : VendrSerializerBase<EmailTemplateReadOnly>, ISyncSerializer<EmailTemplateReadOnly>
    {
        public EmailTemplateSerializer(IVendrApi vendrApi, IUnitOfWorkProvider uowProvider, ILogger logger)
            : base(vendrApi, uowProvider, logger)
        { }

        protected override SyncAttempt<XElement> SerializeCore(EmailTemplateReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.AddStoreId(item.StoreId);

            node.Add(new XElement(nameof(item.Category), item.Category));

            node.Add(SerailizeList(nameof(item.ToAddresses), "Address", item.ToAddresses));
            node.Add(SerailizeList(nameof(item.BccAddresses), "Address", item.BccAddresses));
            node.Add(SerailizeList(nameof(item.CcAddresses), "Address", item.CcAddresses));

            node.Add(new XElement(nameof(item.SenderAddress), item.SenderAddress));
            node.Add(new XElement(nameof(item.SenderName), item.SenderName));
            node.Add(new XElement(nameof(item.SendToCustomer), item.SendToCustomer));
            
            node.Add(new XElement(nameof(item.Subject), item.Subject));
            node.Add(new XElement(nameof(item.TemplateView), item.TemplateView));

            return SyncAttempt<XElement>.SucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        public override bool IsValid(XElement node)
            => base.IsValid(node)
            && node.GetStoreId() != Guid.Empty;

        protected override SyncAttempt<EmailTemplateReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readOnlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readOnlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.GetStoreId();

            using (var uow = _uowProvider.Create())
            {
                EmailTemplate item;
                if (readOnlyItem == null)
                {
                    item = EmailTemplate.Create(uow, id, storeId, alias, name);
                }
                else
                {
                    item = readOnlyItem.AsWritable(uow);
                    item.SetAlias(alias)
                         .SetName(name);
                }

                item.SetCategory(node.Element(nameof(item.Category)).ValueOrDefault(item.Category));
                item.SetSenderName(node.Element(nameof(item.SenderName)).ValueOrDefault(item.SenderName));
                item.SetSenderAddress(node.Element(nameof(item.SenderAddress)).ValueOrDefault(item.SenderAddress));
                item.SetSendToCustomer(node.Element(nameof(item.SendToCustomer)).ValueOrDefault(item.SendToCustomer));
                item.SetSortOrder(node.Element(nameof(item.SortOrder)).ValueOrDefault(item.SortOrder));
                item.SetSubject(node.Element(nameof(item.Subject)).ValueOrDefault(item.Subject));
                item.SetTemplateView(node.Element(nameof(item.TemplateView)).ValueOrDefault(item.TemplateView));

                item.SetToAddresses(DeserializeList<string>(node, nameof(item.ToAddresses), "Address"));
                item.SetBccAddresses(DeserializeList<string>(node, nameof(item.BccAddresses), "Address"));
                item.SetCcAddresses(DeserializeList<string>(node, nameof(item.CcAddresses), "Address"));

                _vendrApi.SaveEmailTemplate(item);

                uow.Complete();

                return SyncAttempt<EmailTemplateReadOnly>.Succeed(name, item.AsReadOnly(), ChangeType.Import);
            }
        }

        // 

        protected override void DeleteItem(EmailTemplateReadOnly item)
            => _vendrApi.DeleteEmailTemplate(item.Id);

        protected override EmailTemplateReadOnly FindItem(Guid key)
            => _vendrApi.GetEmailTemplate(key);

        protected override EmailTemplateReadOnly FindItem(string alias)
            => null;

        protected override string ItemAlias(EmailTemplateReadOnly item)
            => item.Alias;

        protected override void SaveItem(EmailTemplateReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);
                _vendrApi.SaveEmailTemplate(entity);
                uow.Complete();
            }
        }
    }
}
