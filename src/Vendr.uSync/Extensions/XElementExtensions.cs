using System;
using System.Collections.Generic;
using System.Xml.Linq;
using uSync.Core;

namespace Vendr.uSync.Extensions
{
    public static class XElementExtensions
    {
        /// <summary>
        ///  Return the guid value or a null if there isn't one set.
        /// </summary>
        /// <remarks>
        ///  uSync works mainly in guids that can be Guid.Empty, this
        ///  helper class lets us have nullable guids which vendr uses
        ///  internally.
        /// </remarks>
        public static Guid? GetGuidValue(this XElement node, string name)
        {
            var value = node.Element(name).ValueOrDefault(Guid.Empty);
            if (value == Guid.Empty) return null;
            return value;
        }

        /// <summary>
        ///  gets the store Id from the xml
        /// </summary>
        /// <remarks>
        ///  We do this a lot, so this just makes the code for checking etc nicer.
        /// </remarks>
        public static Guid GetStoreId(this XElement node)
            => node.Element("StoreId").ValueOrDefault(Guid.Empty);

        public static void AddStoreId(this XElement node, Guid storeId)
            => node.Add(new XElement("StoreId", storeId));


        public static XElement AddDictionary(this XElement node, IReadOnlyDictionary<string, string> dictionary)
        {
            if (dictionary != null && dictionary.Count > 0)
            {
                foreach(var item in dictionary)
                {
                    node.Add(new XElement("Value", new XAttribute("Key", item.Key), item.Value));
                }
            }

            return node;
        }

        public static IDictionary<string, string> GetDictionary(this XElement node)
        {
            var dictionary = new Dictionary<string, string>();
            if (node == null || !node.HasElements) return dictionary;

            foreach (var itemNode in node.Elements("Value"))
            {
                var key = itemNode.Attribute("Key").ValueOrDefault(string.Empty);
                var value = itemNode.ValueOrDefault(string.Empty);

                if (!string.IsNullOrEmpty(key))
                    dictionary.Add(key, value);
            }

            return dictionary;
        }
    }
}
