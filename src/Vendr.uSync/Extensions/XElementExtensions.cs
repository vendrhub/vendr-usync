using System;
using System.Xml.Linq;

using uSync8.Core.Extensions;

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
    }
}
