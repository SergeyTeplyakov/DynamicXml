//-----------------------------------------------------------------------------------------------//
// Dynamic wrapper around XAttribute object
//
// Author:    Sergey Teplyakov
// Date:      19/04/2011
//-----------------------------------------------------------------------------------------------//

using System;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Globalization;
using System.Xml.Linq;

namespace DynamicXml
{
    /// <summary>
    /// Dynamic wrapper around XAttribute
    /// </summary>
    public class DynamicXAttribute : DynamicObject
    {
        //---------------------------------------------------------------------------------------//
        // Private Fields
        //---------------------------------------------------------------------------------------//
        private readonly XAttribute attribute;

        //---------------------------------------------------------------------------------------//
        // Construction, destruction
        //---------------------------------------------------------------------------------------//

        private DynamicXAttribute(XAttribute attribute)
        {
            Contract.Requires(attribute != null);
            this.attribute = attribute;
        }

        public static dynamic CreateInstance(XAttribute attribute)
        {
            Contract.Requires(attribute != null);
            Contract.Ensures(Contract.Result<object>() != null);

            return new DynamicXAttribute(attribute);
        }

        //---------------------------------------------------------------------------------------//
        // DynamicObject Overrides
        //---------------------------------------------------------------------------------------//
        
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == typeof(XAttribute))
            {
                result = attribute;
                return true;
            }

            string underlyingValue = attribute.Value;

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

        //---------------------------------------------------------------------------------------//
        // Public Interface
        //---------------------------------------------------------------------------------------//

        public string Value
        {
            get { return attribute.Value; }
        }
    }
}