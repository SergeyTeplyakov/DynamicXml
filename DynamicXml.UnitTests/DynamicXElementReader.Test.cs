using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System;
using System.Xml.Linq;

namespace DynamicXml.UnitTests
{
    /// <summary>
    /// Unit tests for DynamicXElement class.
    /// </summary>
    /// <remarks>
    /// This class splitted into several separate partial classes and each partial class
    /// contains unit tests for some specific feature set for DynamicXElement class.
    /// </remarks>
    [TestFixture]
    public partial class DynamicXElementTest
    {
        //---------------------------------------------------------------------------------------//
        // Construction
        //---------------------------------------------------------------------------------------//

        [TestCase(ExpectedExceptionName = "System.Diagnostics.Contracts.__ContractsRuntime+ContractException")]
        public void CreateInstanceFailureTest()
        {
            dynamic actual = DynamicXElementReader.CreateInstance(null);
        }

        [TestCase]
        public void CreateInstanceTest()
        {
            XElement element = new XElement("name", "value");
            dynamic actual = element.AsDynamic();
            Assert.IsNotNull(actual);
        }

        //---------------------------------------------------------------------------------------//
        // System.Object overrides
        //---------------------------------------------------------------------------------------//

        // All this methods should delegate calls to underlying element methods
        
        [TestCase]
        public void ToStringMethodTest()
        {
            // ToString method should call underlying element.ToString method
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = element.AsDynamic();
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
            dynamic dynamicElement1 = DynamicXElementReader.CreateInstance(element1);
            dynamic dynamicElement2 = DynamicXElementReader.CreateInstance(element2);

            Console.WriteLine("Equals: " + dynamicElement1.Equals(dynamicElement2));
            Assert.That(element1.Equals(element2), 
                    Is.EqualTo(dynamicElement1.Equals(dynamicElement2)));

            Console.WriteLine("Hashcodes: element1: {0}, element2: {1}", element1.GetHashCode(), element2.GetHashCode());
            Assert.That(element1.GetHashCode(), Is.EqualTo(dynamicElement1.GetHashCode()));
            Assert.That(element2.GetHashCode(), Is.EqualTo(dynamicElement2.GetHashCode()));
        }

        [TestCase]
        public void GetDynamicMemberNamesTest()
        {
            XElement element = new XElement("element1",
                new XElement("Subelement1", 1),
                new XElement("Subelement2", 2));
            dynamic dynamicElement = element.AsDynamic();
            IEnumerable<string> memberNames = dynamicElement.GetDynamicMemberNames();

            CollectionAssert.AreEqual(memberNames, new string[] {"Subelement1", "Subelement2"});
        }

[TestCase]
public void SkeetBookXmlTest()
{
string books =
@"<books>
  <book name=""Mortal Engines"">
    <author name=""Philip Reeve"" />
  </book>
  <book name=""The Talisman"">
    <author name=""Stephen King"" />
    <author name=""Peter Straub"" />
  </book>
  <book name=""Rose"">
    <author name=""Holly Webb"" />
    <excerpt>Rose was remembering the illustrations from Morally Instructive Tales for the Nursery.</excerpt>
  </book>
</books>";
    dynamic dynamicXml = XElement.Parse(books).AsDynamic();

    Assert.That(dynamicXml.book[0]["name"].Value, Is.EqualTo("Mortal Engines"));

    Assert.That(dynamicXml.book[0].author["name"].Value, Is.EqualTo("Philip Reeve"));

    Assert.That(dynamicXml.book[2]["name"].Value, Is.EqualTo("Rose"));

    Assert.That((string)dynamicXml.book[2].excerpt, Is.EqualTo("Rose was remembering the illustrations from Morally Instructive Tales for the Nursery."));

}

    }
}
