using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics.Contracts;
using System.Dynamic;

namespace DynamicXml
{
    /// <summary>
    /// "Dynamic wrapper" around XElement.
    /// </summary>
    public class DynamicXElement : DynamicObject
    {
        //---------------------------------------------------------------------------------------//
        // Private Fields
        //---------------------------------------------------------------------------------------//
        private XElement element;

        //---------------------------------------------------------------------------------------//
        // Construction, Destruction
        //---------------------------------------------------------------------------------------//
        
        #region Construction, Destruction
        
        private DynamicXElement(XElement element)
        {
            Contract.Requires(element != null);
            this.element = element;
        }

        /// <summary>
        /// Factory made intended usage more clear. We "should" use object of this class dynamically.
        /// </summary>
        public static dynamic CreateInstance(XElement element)
        {
            Contract.Requires(element != null);
            return new DynamicXElement(element);
        }

        #endregion Construction, Destruction

        //---------------------------------------------------------------------------------------//
        // System.Object overrides
        //---------------------------------------------------------------------------------------//
        
        #region System.Object overrides
        // All overrides for System.Object methods simply delegates they calls to underlying element

        public override string ToString()
        {
            return element.ToString();
        }
        
        public override bool Equals(object obj)
        {
            DynamicXElement rhs = obj as DynamicXElement;
            if (ReferenceEquals(null, rhs))
                return false;
            return element == rhs.element;
        }

        public override int GetHashCode()
        {
            return element.GetHashCode();
        }
        #endregion System.Object overrides

        //---------------------------------------------------------------------------------------//
        // DynamicObject Overrides
        //---------------------------------------------------------------------------------------//
        
        #region DynamicObject Overrides

        /// <summary>
        /// Converting DynamicXElement to any other type means converting (or extracting) underlying
        /// XElement.Value
        /// </summary>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            // We're returning underlying XElement if required
            if (binder.ReturnType == typeof(XElement))
            {
                result = element;
                return true;
            }

            string underlyingValue = element.Value;
            // If this conversion succeeded ok, otherwise we'll have exception
            result = Convert.ChangeType(underlyingValue, binder.ReturnType);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string binderName = binder.Name;
            Contract.Assume(binderName != null);
            XElement subelement = element.Element(binderName);
            if (subelement != null)
            {
                result = DynamicXElement.CreateInstance(subelement);
                return true;
            }

            return base.TryGetMember(binder, out result);
        }
        
        #endregion DynamicObject Overrides

        //---------------------------------------------------------------------------------------//
        // Public Interface
        //---------------------------------------------------------------------------------------//

        #region Public Interface

        /// <summary>
        /// Returns true if current element contains parent node
        /// </summary>
        [Pure]
        public bool HasParent()
        {
            return element.Parent != null;
        }

        /// <summary>
        /// Indexer that returns XAttribute by XNode
        /// </summary>
        public XAttribute this[XNode node]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Indexer that returns subelement by element index
        /// </summary>
        public DynamicXElement this[int idx]
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

                XElement subElement = parent.Elements().ElementAt(idx);
                return CreateInstance(subElement);
            }
        }

        #endregion Public Interface

    }
}
