//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Linq;
//using NUnit.Framework;

//namespace DynamicXml.UnitTests
//{
//    public partial class DynamicXElementTest 
//    {
//        //---------------------------------------------------------------------------------------//
//        // Accessing subelements
//        //---------------------------------------------------------------------------------------//
//        [TestCase]
//        public void AccessStringSubelement()
//        {
//            // Separate subelement simplifies testing
//            XElement subelement = new XElement("Subelement", "value");
//            XElement element = new XElement("Element", subelement);
//            dynamic dynamicElement = element.AsDynamic();

//            XElement expectedSubelement = dynamicElement.Subelement;
//            Assert.AreEqual(expectedSubelement, subelement);

//            string value = dynamicElement.Subelement;
//            Assert.That(value, Is.EqualTo("value"));

//            Console.WriteLine("Subelement: {0}", expectedSubelement);
//            Console.WriteLine("Subelement value: {0}", value);
//        }

//        [TestCase]
//        public void AccessEnumerableSubelement()
//        {
//            XElement firstElement = new XElement("Subelement", "value1");
//            XElement secondElement = new XElement("Subelement", "value2");
//            XElement element = new XElement("Element", firstElement, secondElement);

//            dynamic dynamicElement = element.AsDynamic();
//            // We could convert any dynamic XElement wrapper to List of it subelements
//            IEnumerable<XElement> expectedSubelements = dynamicElement;
            
//            IList<XElement> subelements = expectedSubelements.ToList();

//            Assert.That(subelements.Count, Is.EqualTo(2));
            
//            Assert.That(subelements[0], Is.EqualTo(firstElement));

//            Assert.That(subelements[1], Is.EqualTo(secondElement));
//        }
//    }
//}