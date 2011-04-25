//using System.Xml.Linq;
//using NUnit.Framework;

//namespace DynamicXml.UnitTests
//{
//    public partial class DynamicXElementTest 
//    {
//        [TestCase]
//        public void AccessAttributeValueTest()
//        {
//            XElement element = new XElement("name", 
//                new XAttribute("attr_name", "value"));
//            dynamic dynamicElement = element.AsDynamic();

//            XAttribute attribute = dynamicElement["attr_name"];
//            Assert.That(attribute.Value, Is.EqualTo("value"));
//        }


//    }
//}