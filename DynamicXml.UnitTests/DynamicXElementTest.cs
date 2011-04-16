using NUnit.Framework;
using System;
using System.Xml.Linq;

namespace DynamicXml.UnitTests
{
    /// <summary>
    /// Unit tests for DynamicXElement class
    /// </summary>
    [TestFixture]
    public class DynamicXElementTest
    {
        //---------------------------------------------------------------------------------------//
        // Construction
        //---------------------------------------------------------------------------------------//
        #region Construction

        [TestCase(ExpectedExceptionName = "System.Diagnostics.Contracts.__ContractsRuntime+ContractException")]
        public void CreateInstanceFailureTest()
        {
            dynamic actual = DynamicXElement.CreateInstance(null);
        }

        [TestCase]
        public void CreateInstanceTest()
        {
            XElement element = new XElement("name", "value");
            dynamic actual = DynamicXElement.CreateInstance(element);
            Assert.IsNotNull(actual);
        }
        #endregion Construction

        //---------------------------------------------------------------------------------------//
        // System.Object overrides
        //---------------------------------------------------------------------------------------//
        #region System.Object overrides

        // All this methods should delegate calls to underlying element methods
        
        [TestCase]
        public void ToStringMethodTest()
        {
            // ToString method should call underlying element.ToString method
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            string actual = dynamicElement.ToString();
            string expected = element.ToString();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase]
        public void EqualsMethodTest()
        {
            // ToString method should call underlying element.ToString method
            XElement element1 = new XElement("name", "value");
            XElement element2 = new XElement("name", "value");
            dynamic dynamicElement1 = DynamicXElement.CreateInstance(element1);
            dynamic dynamicElement2 = DynamicXElement.CreateInstance(element2);

            Console.WriteLine("Equals: " + dynamicElement1.Equals(dynamicElement2));
            Assert.That(element1.Equals(element2), 
                    Is.EqualTo(dynamicElement1.Equals(dynamicElement2)));

            Console.WriteLine("Hashcodes: element1: {0}, element2: {1}", element1.GetHashCode(), element2.GetHashCode());
            Assert.That(element1.GetHashCode(), Is.EqualTo(dynamicElement1.GetHashCode()));
            Assert.That(element2.GetHashCode(), Is.EqualTo(dynamicElement2.GetHashCode()));
        }

        #endregion System.Object overrides

        //---------------------------------------------------------------------------------------//
        // Accessing value
        //---------------------------------------------------------------------------------------//

        #region Accessing value

        [TestCase]
        public void AccessXElementTest()
        {
            // Cast to XElement should lead to obtaining underlying XElement
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            XElement underlyingElement = dynamicElement;
            Assert.That(object.ReferenceEquals(underlyingElement, element));
        }

        [TestCase("name", 1, Result = 1)]
        public object AccessValueElementTest(string elementName, object value)
        {
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            object actual = dynamicElement;
            return actual;
        }

        [TestCase]
        public void AccessStringValueElementTest()
        {
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            string actual = dynamicElement;
            Assert.That(actual, Is.EqualTo("value"));
            
            Console.WriteLine("Actual string value: {0}", actual);
        }

        [TestCase]
        public void AccessIntValueElementTest()
        {
            XElement element = new XElement("name", 1);
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            int actual = dynamicElement;
            Assert.That(actual, Is.EqualTo(1));

            Console.WriteLine("Actual int value: {0}", actual);
        }

        [TestCase]
        public void AccessDoubleValueElementTest()
        {
            XElement element = new XElement("name", 1f);
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            float actual = dynamicElement;
            Assert.That(actual, Is.EqualTo(1f));

            Console.WriteLine("Actual float value: {0}", actual);
        }

        // TODO: We should use more intellectual way for testing the rest of the underlying types

        //---------------------------------------------------------------------------------------//
        // Accessing value
        //---------------------------------------------------------------------------------------//

        #region Accessing value

        [TestCase]
        public void AccessingZeroIndexTest()
        {
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            // Access to zero element means access to element itself
            string value = dynamicElement[0];
            Assert.That(value, Is.EqualTo("value"));

            Console.WriteLine("Value is: {0}", value);
        }

        [TestCase]
        public void AccessingNonZeroIndexTest()
        {
            XElement element = new XElement("name", 
                new XElement("Subelement", "value1"),
                new XElement("Subelement", "value2"));
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);

            string value1 = dynamicElement.Subelement[0];
            Assert.That(value1, Is.EqualTo("value1"));
            Console.WriteLine("value1 is: {0}", value1);

            string value2 = dynamicElement.Subelement[1];
            Assert.That(value2, Is.EqualTo("value2"));
            Console.WriteLine("value2 is: {0}", value2);
        }

        [TestCase(ExpectedExceptionName = "System.Diagnostics.Contracts.__ContractsRuntime+ContractException")]
        public void AccessingNonZeroIndexForOneElementTest()
        {
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            string value = dynamicElement[1];
            Assert.Fail();
        }

        #endregion Accessing value



        #endregion Accessing value

        //---------------------------------------------------------------------------------------//
        // Accessing subelements
        //---------------------------------------------------------------------------------------//

        #region Accessing subelements

        [TestCase]
        public void AccessStringSubelement()
        {
            XElement subelement = new XElement("Subelement", "value");
            XElement element = new XElement("Element", subelement);
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            XElement expectedSubelement = dynamicElement.Subelement;
            Assert.AreEqual(expectedSubelement, subelement);

            string value = dynamicElement.Subelement;
            Assert.That(value, Is.EqualTo("value"));

            Console.WriteLine("Subelement: {0}", expectedSubelement);
            Console.WriteLine("Subelement value: {0}", value);
        }

        #endregion Accessing subelements
    }
}
