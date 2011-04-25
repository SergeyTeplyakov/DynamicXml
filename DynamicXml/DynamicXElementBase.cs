//-----------------------------------------------------------------------------------------------//
// Base class for "Dynamic wrapper" around XElement
//
// Author:    Sergey Teplyakov
// Date:      19/04/2011
//-----------------------------------------------------------------------------------------------//


using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace DynamicXml
{
    /// <summary>
    /// Base class for "Dynamic wrapper" around XElement
    /// </summary>
    /// <remarks>
    /// Now we have two separate implementation for dynamic xml wrapper: one for reading xml
    /// and another - for reading and writing. So we have some code common for both of them
    /// and this is reasonable to use common base class for both of them.
    /// </remarks>
    public abstract class DynamicXElementBase : DynamicObject
    {
        //---------------------------------------------------------------------------------------//
        // Protected Fields
        //---------------------------------------------------------------------------------------//
        protected readonly XElement element;

        //---------------------------------------------------------------------------------------//
        // Construction, Destruction
        //---------------------------------------------------------------------------------------//

        protected DynamicXElementBase(XElement element)
        {
            Contract.Requires(element != null);
            this.element = element;
        }


        //---------------------------------------------------------------------------------------//
        // System.Object overrides
        //---------------------------------------------------------------------------------------//
        
        #region System.Object overrides
        // All overrides for System.Object methods simply delegates they calls to underlying element

        public sealed override string ToString()
        {
            return element.ToString();
        }
        
        public sealed override bool Equals(object obj)
        {
            var rhs = obj as DynamicXElementBase;
            if (ReferenceEquals(null, rhs))
                return false;
            return element == rhs.element;
        }

        public sealed override int GetHashCode()
        {
            return element.GetHashCode();
        }
        #endregion System.Object overrides

        //---------------------------------------------------------------------------------------//
        // DynamicObject Overrides
        //---------------------------------------------------------------------------------------//
        
        #region DynamicObject Overrides

        /// <summary>
        /// Converting dynamic XElement wrapper to any other type means converting (or extracting) 
        /// underlying  element.Value
        /// </summary>
        public override sealed bool TryConvert(ConvertBinder binder, out object result)
        {
            // We're returning underlying XElement if required
            if (binder.ReturnType == typeof(XElement))
            {
                result = element;
                return true;
            }

            // We could treat dynamic wrapper as collection of it subelements
            if (binder.ReturnType.IsAssignableFrom(typeof(IEnumerable<XElement>)))
            {
                result = element.Elements();
                return true;
            }

            string underlyingValue = element.Value;

            // We shoult treat TimeSpan separately, because we should call Parse method
            // instead of Convert.ChangeType
            if (binder.ReturnType == typeof(TimeSpan))
            {
                result = TimeSpan.Parse(underlyingValue);
                return true;
            }

            // For the rest of the types we could use Convert.ChangeType
            // If this conversion succeeded ok, otherwise we'll have exception
            result = Convert.ChangeType(underlyingValue, binder.ReturnType, CultureInfo.InvariantCulture);
            return true;
        }

        public sealed override IEnumerable<string> GetDynamicMemberNames()
        {
            return element.Elements()
                          .Select(x => x.Name.LocalName)
                          .Distinct()
                          .OrderBy(x => x);
        }

        
        #endregion DynamicObject Overrides

        //---------------------------------------------------------------------------------------//
        // Public Interface
        //---------------------------------------------------------------------------------------//

        #region Public Interface

        public XElement XElement { get { return element; } }

        /// <summary>
        /// Returns true if current element contains parent node
        /// </summary>
        [Pure]
        public bool HasParent()
        {
            return element.Parent != null;
        }

        #endregion Public Interface

    }
}