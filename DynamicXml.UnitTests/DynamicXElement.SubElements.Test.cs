using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace DynamicXml.UnitTests
{
    public partial class DynamicXElementTest 
    {
        //---------------------------------------------------------------------------------------//
        // Accessing subelements
        //---------------------------------------------------------------------------------------//
        [TestCase]
        public void AccessStringSubelement()
        {
            // Separate subelement simplifies testing
            XElement subelement = new XElement("Subelement", "value");
            XElement element = new XElement("Element", subelement);
            dynamic dynamicElement = element.AsDynamic();

            XElement expectedSubelement = dynamicElement.Subelement;
            Assert.AreEqual(expectedSubelement, subelement);

            string value = dynamicElement.Subelement;
            Assert.That(value, Is.EqualTo("value"));

            Console.WriteLine("Subelement: {0}", expectedSubelement);
            Console.WriteLine("Subelement value: {0}", value);
        }

        [TestCase]
        public void AccessEnumerableSubelement()
        {
            XElement subelement = new XElement("Subelement", "value");
            XElement element = new XElement("Element", 
                new XElement("Subelement", "value1"),
                new XElement("Subelement", "value2"));

            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            XElement expectedSubelement = dynamicElement.Subelement;
            Assert.AreEqual(expectedSubelement, subelement);

            string value = dynamicElement.Subelement;
            Assert.That(value, Is.EqualTo("value"));

            Console.WriteLine("Subelement: {0}", expectedSubelement);
            Console.WriteLine("Subelement value: {0}", value);
        }
    }
}