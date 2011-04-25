using System.Xml.Linq;
using NUnit.Framework;

namespace DynamicXml.UnitTests
{
    public partial class DynamicXElementTest 
    {
        [TestCase]
        public void AccessAttributeValueTest()
        {
            XElement element = new XElement("name", 
                new XAttribute("attr_name", "value"));
            dynamic dynamicElement = element.AsDynamic();

            XAttribute attribute = dynamicElement["attr_name"];
            Assert.That(attribute.Value, Is.EqualTo("value"));
        }

        [TestCase]
        public void AccessAttributeStringValueTest()
        {
            XElement element = new XElement("name",
                new XAttribute("attr_name", "value"));
            dynamic dynamicElement = element.AsDynamic();

            string value = (string)dynamicElement["attr_name"];
            Assert.That(value, Is.EqualTo("value"));
        }

        [TestCase]
        public void AccessAttributeStringValueWithImplicitCastCastTest()
        {
            XElement element = new XElement("name",
                new XAttribute("attr_name", "value"));
            dynamic dynamicElement = element.AsDynamic();

            string value = dynamicElement["attr_name"];
            Assert.That(value, Is.EqualTo("value"));
        }
    }
}