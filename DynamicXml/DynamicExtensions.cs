using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace DynamicXml
{
    /// <summary>
    /// Static class with extensions methods for creating DynamixXElements from XElement.
    /// </summary>
    public static class DynamicExtensions
    {
        /// <summary>
        /// Helper method that creates dynamic wrapper around XElement
        /// </summary>
        public static dynamic AsDynamic(this XElement element)
        {
            Contract.Requires(element != null);
            Contract.Ensures(Contract.Result<object>() != null);

            return DynamicXElement.CreateInstance(element);
        }

        /// <summary>
        /// Helper method that creates dynamic wrapper around XDocument.Root
        /// </summary>
        public static dynamic AsDynamic(this XDocument document)
        {
            Contract.Requires(document != null);
            Contract.Requires(document.Root != null);
            Contract.Ensures(Contract.Result<object>() != null);

            return document.Root.AsDynamic();
        }
    }
}