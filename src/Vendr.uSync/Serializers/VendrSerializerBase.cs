using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Umbraco.Core;
using Umbraco.Core.Logging;

using uSync8.Core.Serialization;

using Vendr.Core;
using Vendr.Core.Api;
using Vendr.Core.Models;

namespace Vendr.uSync.Serializers
{
    /// <summary>
    ///  base for vendr Serializers for uSync. 
    /// </summary>
    public abstract class VendrSerializerBase<TObject>
        : SyncSerializerRoot<TObject>
        where TObject : EntityBase
    {
        protected IVendrApi _vendrApi;
        protected IUnitOfWorkProvider _uowProvider;

        protected VendrSerializerBase(
            IVendrApi vendrApi,
            IUnitOfWorkProvider uowProvider,
            ILogger logger) : base(logger)
        {
            _vendrApi = vendrApi;
            _uowProvider = uowProvider;
        }

        protected override Guid ItemKey(TObject item)
            => item.Id;

        protected override TObject FindItem(string alias)
            => null;


        protected XElement SerailizeList<TResult>(string collectionName, string elementName, IEnumerable<TResult> items)
        {
            var root = new XElement(collectionName);

            if (items.Any())
            {
                foreach (var item in items)
                {
                    root.Add(new XElement(elementName, item));
                }
            }

            return root;
        }

        protected IEnumerable<TResult> DeserializeList<TResult>(XElement node, string collectionName, string elementName)
        {
            var root = node.Element(collectionName);
            if (root == null || !root.HasElements)
                return Enumerable.Empty<TResult>();

            var items = new List<TResult>();
            foreach (var item in root.Elements(elementName))
            {
                var attempt = item.Value.TryConvertTo<TResult>();
                if (attempt.Success)
                    items.Add(attempt.Result);
            }

            return items;
        }
    }
}
