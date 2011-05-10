//-----------------------------------------------------------------------------------------------//
// "Dynamic wrapper" around XElement for reading and writing xml content dynamically
//
// Author:    Sergey Teplyakov
// Date:      19/04/2011
//-----------------------------------------------------------------------------------------------//

using System;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace DynamicXml
{
    /// <summary>
    /// "Dynamic wrapper" around XElement for reading and writing xml content dynamically
    /// </summary>
    /// <remarks>
    /// We should distinqush reader and writer classes due to following issue: reader should
    /// throw exception accessing missing subelement, but writer should create this subelement
    /// instead.
    /// And because we don't know exactly what user wants from this line of code:
    /// dynamicElement.Element.Subelement whether he wants to read or write we should use two
    /// separate classes
    /// </remarks>
    public class DynamicXElementWriter : DynamicXElementBase
    {
        //---------------------------------------------------------------------------------------//
        // Construction, Destruction
        //---------------------------------------------------------------------------------------//

        private DynamicXElementWriter(XElement element)
            : base(element)
        {
            Contract.Requires(element != null);
        }

        /// <summary>
        /// Factory method made intended usage more clear because this class should be used 
        /// only as dynamic.
        /// </summary>
        public static dynamic CreateInstance(XElement element)
        {
            Contract.Requires(element != null);
            return new DynamicXElementWriter(element);
        }

        //---------------------------------------------------------------------------------------//
        // DynamicObject Overrides
        //---------------------------------------------------------------------------------------//
        
        #region DynamicObject Overrides

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string binderName = binder.Name;
            Contract.Assume(binderName != null);
            XElement subelement = element.Element(binderName);
            
            // If we don't have appropriate subelement we'll create it and add to 
            // underlying XElement
            if (subelement == null)            
            {
                subelement = new XElement(binderName);
                element.Add(subelement);
            }

            result = CreateInstance(subelement);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Contract.Assume(binder != null);
            Contract.Assume(!string.IsNullOrEmpty(binder.Name));
            Contract.Assume(value != null);

            string binderName = binder.Name;

            if (binderName == element.Name)
                element.SetValue(value);
            else
                element.SetElementValue(binderName, value);
            return true;
        }

        #endregion DynamicObject Overrides

        /// <summary>
        /// Changing current XElement's value
        /// </summary>
        public void SetValue(object value)
        {
            Contract.Requires(value != null);

            element.SetValue(value);
        }

        /// <summary>
        /// Changing curent XElement's attribute value
        /// </summary>
        public void SetAttributeValue(XName name, object value)
        {
            Contract.Requires(name != null);
            Contract.Requires(value != null);

            element.SetAttributeValue(name, value);
        }

        //---------------------------------------------------------------------------------------//
        // Public Interface
        //---------------------------------------------------------------------------------------//

        #region Public Interface

        /// <summary>
        /// Indexer that returns XAttribute by XNode
        /// </summary>
        public dynamic this[XName name]
        {
            get
            {
                Contract.Requires(name != null);

                XAttribute attribute = element.Attribute(name);

                if (attribute == null)
                    throw new InvalidOperationException("Attribute not found. Name: " + name.LocalName);

                return DynamicXAttribute.CreateInstance(attribute);
            }

            set
            {
                element.SetAttributeValue(name, value);
            }

        }

        /// <summary>
        /// Indexer that returns subelement by element index
        /// </summary>
        public dynamic this[int idx]
        {
            get
            {
                
                Contract.Requires(idx >= 0, "Index should be greater or equals to 0");
                Contract.Requires(idx == 0 || HasParent(), "For non-zero index we should have parent element");

                // For 0 index we returning current element);
                if (idx == 0)
                    return this;
                
                // For non-zero index we'll return appropriate peer.
                // We should take parent and then access to appropriate child element
                var parent = element.Parent;
                Contract.Assume(parent != null);

                
                // TODO: consider throw IndexOutOfRangeException if idx > parent.Elements.Count() - 1
                // Lets add new element if appropriate element doesn't exist
                XElement subElement = parent.Elements(element.Name).ElementAtOrDefault(idx);
                if (subElement == null)
                {
                    // Otherwise we should add another element to our parent
                    XElement sibling = parent.Elements(element.Name).First();
                    subElement = new XElement(sibling.Name);
                    parent.Add(subElement);
                }

                //Contract.Assume(subElement != null);
                return CreateInstance(subElement);
            }

            set
            {
                Contract.Requires(idx >= 0, "Index should be greater or equals to 0");
                Contract.Requires(idx == 0 || HasParent(), "For non-zero index we should have parent element");
                // TODO: consider use more strict postcondition
                // Contract.Ensures(((string)this[idx]).Equals((object)value.ToString()));

                // Let's implement setter through getter
                dynamic d = this[idx];
                d.SetValue(value);
                return;
            }

        }

        #endregion Public Interface
    }
}