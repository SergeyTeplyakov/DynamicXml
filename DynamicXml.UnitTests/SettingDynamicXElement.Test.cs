using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace DynamicXml.UnitTests
{
    /// <summary>
    /// Unit tests for changing XElement through dynamic wrapper
    /// </summary>
    [TestFixture]
    public class SettingDynamicXElementTest
    {
        [TestCase]
        public void ChangeStringValueTest()
        {
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = element.AsDynamic();
            dynamicElement.name = "value2";

            Assert.That(element.Value, Is.EqualTo("value2"));
        }

        [TestCase]
        public void ChangeValueAndTypeTest()
        {
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = element.AsDynamic();
            dynamicElement.SetValue(123);
            Assert.That(element.Value, Is.EqualTo("123"));
        }

        [TestCase]
        public void InsertSubelementTest()
        {
            XElement element = new XElement("name");
            dynamic dynamicElement = element.AsDynamic();
            
            dynamicElement.Subelement.SubSubelement = 123;
            int subelementValue = dynamicElement.Subelement.SubSubElement;

            Assert.That(subelementValue, Is.EqualTo(123));
            XElement subelement = element.Element("Subelement");
            Assert.That(subelement, Is.Not.Null);
            Assert.That(subelement.Value, Is.EqualTo("123"));
            Console.WriteLine("Changed element: {0}", element);
        }

        [TestCase]
        public void ChangeAttributeTest()
        {
            XElement element = new XElement("name",
                new XAttribute("attr", "value"));
            dynamic dynamicElement = element.AsDynamic();
            dynamicElement.SetAttributeValue("attr",  "value2");

            string attributeValue = element.Attribute("attr").Value;
            Assert.That(attributeValue, Is.EqualTo("value2"));
        }

        [TestCase]
        public void AddAttributeTest()
        {
            XElement element = new XElement("name");
            dynamic dynamicElement = element.AsDynamic();
            dynamicElement.SetAttributeValue("attr", 1231);

            string attributeValue = element.Attribute("attr").Value;
            Assert.That(attributeValue, Is.EqualTo("1231"));
        }

    }
}