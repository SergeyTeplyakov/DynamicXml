//-----------------------------------------------------------------------------------------------//
// "Dynamic wrapper" around XElement for reading xml content dynamically
//
// Author:    Sergey Teplyakov
// Date:      19/04/2011
//-----------------------------------------------------------------------------------------------//

using System;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics.Contracts;
using System.Dynamic;

namespace DynamicXml
{
    /// <summary>
    /// "Dynamic wrapper" around XElement for reading xml content dynamically
    /// </summary>
    /// <remarks>
    /// This class intended only for reading operations, i.e. we can't modify underlying
    /// XElement with this class.
    /// If you want also modify underlying element or build xml data from scratch you
    /// should use <see cref="DynamicXElementWriter"/>.
    /// </remarks>
    public class DynamicXElementReader : DynamicXElementBase
    {
        //---------------------------------------------------------------------------------------//
        // Construction, Destruction
        //---------------------------------------------------------------------------------------//

        private DynamicXElementReader(XElement element)
            : base(element)
        {
            Contract.Requires(element != null);
        }

        /// <summary>
        /// Factory made intended usage more clear. We "should" use object of this class dynamically.
        /// </summary>
        public static dynamic CreateInstance(XElement element)
        {
            Contract.Requires(element != null);
            Contract.Ensures(Contract.Result<object>() != null);

            return new DynamicXElementReader(element);
        }

        //---------------------------------------------------------------------------------------//
        // DynamicObject Overrides
        //---------------------------------------------------------------------------------------//
        
        #region DynamicObject Overrides

        /// <summary>
        /// This method called during access to underlying subelement
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string binderName = binder.Name;
            Contract.Assume(binderName != null);

            // Finding apprpopriate subelement and creating dynamic wrapper
            // if this subelement exists
            XElement subelement = element.Element(binderName);
            if (subelement != null)
            {
                result = CreateInstance(subelement);
                return true;
            }
            
            // Calling base implementation leads to runtime exception
            return base.TryGetMember(binder, out result);
        }

        #endregion DynamicObject Overrides

        //---------------------------------------------------------------------------------------//
        // Public Interface
        //---------------------------------------------------------------------------------------//

        #region Public Interface

        /// <summary>
        /// Indexer that returns XAttribute wrapper by XNode
        /// </summary>
        /// <example>
        /// You have several choises how to deal with attributes.
        /// 
        /// // 1
        /// XElement element = new XElement("name", new XAttribute("attr", "val"));
        /// dynamic dynamicElement = element.AsDynamic();
        /// string value = dynamicElement["attr"];
        /// 
        /// // 2
        /// XElement element = new XElement("name", new XAttribute("attr", "val"));
        /// dynamic dynamicElement = element.AsDynamic();
        /// XAttribute attribute = dynamicElement["attr"];
        /// string value = attribute.Value;
        /// string theSameValue = (string)attribute;
        /// 
        /// // 3
        /// XElement element = new XElement("name", new XAttribute("attr", "val"));
        /// dynamic dynamicElement = element.AsDynamic();
        /// string value = (string)dynamicElement["attr"];
        /// </example>
        public dynamic this[XName name]
        {
            get
            {
                Contract.Requires(name != null);

                XAttribute attribute = element.Attribute(name);

                if (attribute == null)
                    throw new InvalidOperationException("Attribute not found. Name: " + name.LocalName);

                return attribute.AsDynamic();
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

                // For 0 index we returning current element
                if (idx == 0)
                    return this;
                
                // For non-zero index we'll return appropriate peer.
                // We should take parent and then access to appropriate child element
                var parent = element.Parent;
                Contract.Assume(parent != null);

                XElement subElement = parent.Elements().ElementAt(idx);

                // subElement can't be null. If we don't have an element with appropriate index, we'll have
                // ArgumentOutOfRangeException. Lets suggest this to Static Code analyzer
                Contract.Assume(subElement != null);

                return CreateInstance(subElement);
            }

        }

        #endregion Public Interface

    }
}
