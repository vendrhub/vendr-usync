using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Microsoft.Extensions.Logging;

using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;

using Vendr.Common;
using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Extensions;
using Vendr.uSync.Configuration;
using Vendr.uSync.Extensions;

namespace Vendr.uSync.Serializers;

[SyncSerializer("43AD88B3-AB60-438E-A8E8-30223CFD739C", "Product Attribute Serializer",
    VendrConstants.Serialization.ProductAttributes)]
public class ProductAttributesSerializer : VendrSerializerBase<ProductAttributeReadOnly>, ISyncSerializer<ProductAttributeReadOnly>
{
    public ProductAttributesSerializer(
        IVendrApi vendrApi,
        VendrSyncSettingsAccessor settingsAccessor,
        IUnitOfWorkProvider uowProvider,
        ILogger<VendrSerializerBase<ProductAttributeReadOnly>> logger) 
        : base(vendrApi, settingsAccessor, uowProvider, logger)
    { }

    public override string GetItemAlias(ProductAttributeReadOnly item) 
        => item.Alias;

    protected override SyncAttempt<XElement> SerializeCore(ProductAttributeReadOnly item, SyncSerializerOptions options)
    {
        var node = InitializeBaseNode(item, ItemAlias(item));


        node.Add(new XElement(nameof(item.Name), 
            new XAttribute("Default", item.Name.GetDefaultValue()))
            .AddDictionary(item.Name.GetTranslatedValues()));

        node.Add(new XElement(nameof(item.SortOrder), item.SortOrder));
        node.AddStoreId(item.StoreId);

        node.Add(SerializeAttributeValues(item.Values));

        return SyncAttemptSucceedIf(node != null, item.Name, node, ChangeType.Export);
    }

    private XElement SerializeAttributeValues(IReadOnlyCollection<ProductAttributeValueReadOnly> values)
    {
        var valuesNode = new XElement("Values");
        if (values == null) return valuesNode;

        foreach(var value in values)
        {
            var valueNode = new XElement("Value");
            valueNode.Add(new XElement(nameof(value.Alias), value.Alias));

            valueNode.Add(new XElement(nameof(value.Name),
                   new XAttribute("Default", value.Name.GetDefaultValue()))
                   .AddDictionary(value.Name.GetTranslatedValues()));

            valuesNode.Add(valueNode);
        }

        return valuesNode;
    }

    public override bool IsValid(XElement node)
        => base.IsValid(node)
        && node.GetStoreId() != Guid.Empty;

    protected override SyncAttempt<ProductAttributeReadOnly> DeserializeCore(XElement node, SyncSerializerOptions options)
    {
        var readonlyitem = FindItem(node);

        var alias = node.GetAlias();
        var id = node.GetKey();
        var defaultName = node.Element(nameof(readonlyitem.Name)).Attribute("Default").ValueOrDefault(string.Empty);
        var translatedNames = node.Element(nameof(readonlyitem.Name)).GetDictionary();
        var storeId = node.GetStoreId();

        return _uowProvider.Execute(uow =>
        {
            ProductAttribute item;

            if (readonlyitem == null)
            {
                item = ProductAttribute.Create(uow, storeId, alias, defaultName);
            }
            else
            {
                item = readonlyitem.AsWritable(uow);
            }

            item.SetAlias(alias)
                .SetName(new TranslatedValue<string>(defaultName, translatedNames))
                .SetSortOrder(node.Element(nameof(item.SortOrder)).ValueOrDefault(item.SortOrder));

            DeserializeAttributeValues(node, item);

            return uow.Complete(SyncAttemptSucceed(defaultName, item.AsReadOnly(), ChangeType.Import));
        });
    }

    private void DeserializeAttributeValues(XElement node, ProductAttribute item)
    {
        var attributeValues = new Dictionary<string, TranslatedValue<string>>();

        var valuesNode = node.Element("Values");
        if (valuesNode != null && valuesNode.HasElements)
        {
            foreach(var valueNode in valuesNode.Elements("Value"))
            {
                var alias = valueNode.Element("Alias").ValueOrDefault(string.Empty);
                var ProductAttributeId = valueNode.Element("ProductAttributeId").ValueOrDefault(Guid.Empty);

                var defaultName = valueNode.Element("Name").Attribute("Default").ValueOrDefault(string.Empty);
                var translatedNames = valueNode.Element("Name").GetDictionary();

                if (!string.IsNullOrWhiteSpace(alias))
                {
                    attributeValues.Add(alias, new TranslatedValue<string>(defaultName, translatedNames));
                }
            }
        }

        item.SetValues(attributeValues);
    }


    public override void DoDeleteItem(ProductAttributeReadOnly item)
       => _vendrApi.DeleteProductAttribute(item.Id);

    public override ProductAttributeReadOnly DoFindItem(Guid key)
        => _vendrApi.GetProductAttribute(key);

    public override void DoSaveItem(ProductAttributeReadOnly item)
    {
        _uowProvider.Execute(uow =>
        {
            var entity = item.AsWritable(uow);
            _vendrApi.SaveProductAttribute(entity);
            uow.Complete();
        });
    }

}
