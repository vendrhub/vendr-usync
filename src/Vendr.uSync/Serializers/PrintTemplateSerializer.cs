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
    [SyncSerializer("D0D7176C-2EDD-453E-9795-D71F1D29B44A", "Print Template Serializer", VendrConstants.Serialization.PrintTemplate)]
    public class PrintTemplateSerializer : VendrSerializerBase<PrintTemplateReadOnly>, ISyncSerializer<PrintTemplateReadOnly>
    {
        public PrintTemplateSerializer(IVendrApi vendrApi, IUnitOfWorkProvider uowProvider, ILogger logger)
            : base(vendrApi, uowProvider, logger)
        { }

        protected override SyncAttempt<XElement> SerializeCore(PrintTemplateReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.AddStoreId(item.StoreId);

            node.Add(new XElement(nameof(item.Category), item.Category));
            node.Add(new XElement(nameof(item.TemplateView), item.TemplateView));

            return SyncAttempt<XElement>.SucceedIf(node != null, item.Name, node, ChangeType.Export);
        }

        public override bool IsValid(XElement node)
            => base.IsValid(node)
            && node.GetStoreId() != Guid.Empty;

        protected override SyncAttempt<PrintTemplateReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
        {
            var readOnlyItem = FindItem(node);

            var alias = node.GetAlias();
            var id = node.GetKey();
            var name = node.Element(nameof(readOnlyItem.Name)).ValueOrDefault(alias);
            var storeId = node.GetStoreId();

            using (var uow = _uowProvider.Create())
            {
                PrintTemplate item;
                if (readOnlyItem == null)
                {
                    item = PrintTemplate.Create(uow, id, storeId, alias, name);
                }
                else
                {
                    item = readOnlyItem.AsWritable(uow);
                    item.SetAlias(alias)
                         .SetName(name);
                }

                item.SetCategory(node.Element(nameof(item.Category)).ValueOrDefault(item.Category));
                item.SetTemplateView(node.Element(nameof(item.TemplateView)).ValueOrDefault(item.TemplateView));

                _vendrApi.SavePrintTemplate(item);

                uow.Complete();

                return SyncAttempt<PrintTemplateReadOnly>.Succeed(name, item.AsReadOnly(), ChangeType.Import);
            }
        }

        // 

        protected override void DeleteItem(PrintTemplateReadOnly item)
            => _vendrApi.DeletePrintTemplate(item.Id);

        protected override PrintTemplateReadOnly FindItem(Guid key)
            => _vendrApi.GetPrintTemplate(key);

        protected override PrintTemplateReadOnly FindItem(string alias)
            => null;

        protected override string ItemAlias(PrintTemplateReadOnly item)
            => item.Alias;

        protected override void SaveItem(PrintTemplateReadOnly item)
        {
            using (var uow = _uowProvider.Create())
            {
                var entity = item.AsWritable(uow);
                _vendrApi.SavePrintTemplate(entity);
                uow.Complete();
            }
        }
    }
}
