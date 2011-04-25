//-----------------------------------------------------------------------------------------------//
// Static class with extensions methods for creating dynamic wrappers for XElement, XDocument and 
// XAttribute
//
// Author:    Sergey Teplyakov
// Date:      19/04/2011
//-----------------------------------------------------------------------------------------------//


using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace DynamicXml
{
    /// <summary>
    /// Static class with extensions methods for creating dynamic wrappers for XElement, XDocument and 
    /// XAttribute
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

            return DynamicXElementReader.CreateInstance(element);
        }

        /// <summary>
        /// Creates dynamic wrapper around XDocument.Root
        /// </summary>
        public static dynamic AsDynamic(this XDocument document)
        {
            Contract.Requires(document != null);
            Contract.Requires(document.Root != null);
            Contract.Ensures(Contract.Result<object>() != null);

            return document.Root.AsDynamic();
        }


        /// <summary>
        /// Creates dynamic wrapper that could read and write underlying XElement
        /// </summary>
        public static dynamic AsDynamicWriter(this XElement element)
        {
            Contract.Requires(element != null);
            Contract.Ensures(Contract.Result<object>() != null);

            return DynamicXElementWriter.CreateInstance(element);
        }

        /// <summary>
        /// Creates dynamic wrapper around XAttribute
        /// </summary>
        public static dynamic AsDynamic(this XAttribute attribute)
        {
            Contract.Requires(attribute != null);
            Contract.Ensures(Contract.Result<object>() != null);

            return DynamicXAttribute.CreateInstance(attribute);
        }
    }
}