﻿using System;
using System.Xml.Linq;

using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Common;

using Vendr.uSync.Extensions;
using Vendr.uSync.Configuration;

using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;
using Microsoft.Extensions.Logging;
using Vendr.Extensions;

namespace Vendr.uSync.Serializers
{
    [SyncSerializer("D0D7176C-2EDD-453E-9795-D71F1D29B44A", "Print Template Serializer", VendrConstants.Serialization.PrintTemplate)]
    public class PrintTemplateSerializer : VendrSerializerBase<PrintTemplateReadOnly>, ISyncSerializer<PrintTemplateReadOnly>
    {
        public PrintTemplateSerializer(IVendrApi vendrApi, VendrSyncSettingsAccessor settingsAccessor,
            IUnitOfWorkProvider uowProvider,
            ILogger<PrintTemplateSerializer> logger) : base(vendrApi, settingsAccessor, uowProvider, logger)
        { }

        protected override SyncAttempt<XElement> SerializeCore(PrintTemplateReadOnly item, SyncSerializerOptions options)
        {
            var node = InitializeBaseNode(item, ItemAlias(item));

            node.Add(new XElement(nameof(item.Name), item.Name));
            node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
            node.AddStoreId(item.StoreId);

            node.Add(new XElement(nameof(item.Category), item.Category));
            node.Add(new XElement(nameof(item.TemplateView), item.TemplateView));

            return SyncAttemptSucceedIf(node != null, item.Name, node, ChangeType.Export);
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

            return _uowProvider.Execute(uow =>
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

                return uow.Complete(SyncAttemptSucceed(name, item.AsReadOnly(), ChangeType.Import));
            });
        }

        // 

        public override string GetItemAlias(PrintTemplateReadOnly item)
            => item.Alias;

        public override void DoDeleteItem(PrintTemplateReadOnly item)
            => _vendrApi.DeletePrintTemplate(item.Id);

        public override PrintTemplateReadOnly DoFindItem(Guid key)
            => _vendrApi.GetPrintTemplate(key);

        public override PrintTemplateReadOnly DoFindItem(string alias)
            => null;

        public override void DoSaveItem(PrintTemplateReadOnly item)
        {
            _uowProvider.Execute(uow =>
            {
                var entity = item.AsWritable(uow);

                _vendrApi.SavePrintTemplate(entity);

                uow.Complete();
            });
        }
    }
}
