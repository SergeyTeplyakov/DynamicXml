using System.Collections.Generic;
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
    public partial class DynamicXElementWriterTest
    {
        //---------------------------------------------------------------------------------------//
        // Construction
        //---------------------------------------------------------------------------------------//

        [TestCase(ExpectedExceptionName = "System.Diagnostics.Contracts.__ContractsRuntime+ContractException")]
        public void CreateInstanceFailureTest()
        {
            dynamic actual = DynamicXElementWriter.CreateInstance(null);
        }

        [TestCase]
        public void ChangeExistingElementTest()
        {
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = element.AsDynamicWriter();
            // Unfortunately we can't use dynamicElement = "value2", because this leads to assigning
            // new value to dynamic object
            dynamicElement.SetValue("value2");

            Assert.That(element.Value, Is.EqualTo("value2"));
        }

        [TestCase]
        public void ChangeExistingSubelementTest()
        {
            XElement element = new XElement("UserInfo",
                new XElement("Name", "John"));
            dynamic dynamicElement = element.AsDynamicWriter();
            dynamicElement.Name = "Bob";

            string name = dynamicElement.Name;
            Assert.That(name, Is.EqualTo("Bob"));
        }

        [TestCase]
        public void InsertingSubelementTest()
        {
            XElement element = new XElement("UserInfo");
            dynamic dynamicElement = element.AsDynamicWriter();
            dynamicElement.Name = "John";

            Console.WriteLine("Resulting XElement: {0}", element);

            Assert.That(element.Element("Name").Value, Is.EqualTo("John"));
            string name = dynamicElement.Name;
            Assert.That(name, Is.EqualTo("John"));
        }

        [TestCase]
        public void InsertingSeveralSubelementsTest()
        {
            XElement element = new XElement("Users");
            dynamic dynamicElement = element.AsDynamicWriter();

            dynamicElement.User[0] = "User1";
            dynamicElement.User[1] = "User2";
            
            Console.WriteLine("Resulting XElement: {0}", element);

            string user1 = dynamicElement.User[0];
            string user2 = dynamicElement.User[1];
        
            Assert.That(user1, Is.EqualTo("User1"));
            Assert.That(user2, Is.EqualTo("User2"));
        }


        [TestCase]
        public void InsertSeveralSubelementsTest()
        {
            XElement element = new XElement("UserInfo");
            dynamic dynamicElement = element.AsDynamicWriter();
            dynamicElement.Name.SubName.SubSubName.AnotherProperty = 123;

            Console.WriteLine("Resulting XElement: {0}", element);

            XElement subelement =
                element.Element("Name").Element("SubName").Element("SubSubName").Element("AnotherProperty");

            Assert.That((int)subelement, Is.EqualTo(123));

        }

        [TestCase]
        public void ChangeExistingAttributeTest()
        {
            XElement element = new XElement("UserInfo",
                new XAttribute("Name", "John"));
            dynamic dynamicElement = element.AsDynamicWriter();
            dynamicElement["Name"] = "Bob";

            Console.WriteLine("Changed element: {0}", element);

            string name = dynamicElement["Name"];
            Assert.That(name, Is.EqualTo("Bob"));
        }

        [TestCase]
        public void CreatingNewAttributeTest()
        {
            XElement element = new XElement("UserInfo");
            dynamic dynamicElement = element.AsDynamicWriter();
            dynamicElement["Name"] = "Bob";

            Console.WriteLine("Changed element: {0}", element);

            string name = dynamicElement["Name"];
            Assert.That(name, Is.EqualTo("Bob"));
        }

        [TestCase]
        public void SkeetBookXmlTest()
        {
            // Jon Skeet in his C# in Depth used following sample
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
            // Lets create this data dynamically
            XElement element = new XElement("books");
            dynamic dynamicXml = element.AsDynamicWriter();

            dynamicXml.book[0]["name"] = "Mortal Engines";
            dynamicXml.book[0].author["name"] = "Philip Reeve";

            dynamicXml.book[1]["name"] = "The Tailisman";
            dynamicXml.book[1].author[0]["name"] = "Stephen King";
            dynamicXml.book[1].author[1]["name"] = "Peter Straub";

            dynamicXml.book[2]["name"] = "Rose";
            dynamicXml.book[2].author["name"] = "Holly Webb";
            dynamicXml.book[2].excerpt = "Rose was remembering the illustrations from Morally Instructive Tales for the Nursery.";


            Console.WriteLine(element);
            //dynamic dynamicXml = XElement.Parse(books).AsDynamic();
            
            Assert.That(dynamicXml.book[0]["name"].Value, Is.EqualTo("Mortal Engines"));

            Assert.That(dynamicXml.book[0].author["name"].Value, Is.EqualTo("Philip Reeve"));

            Assert.That(dynamicXml.book[2]["name"].Value, Is.EqualTo("Rose"));

            Assert.That((string)dynamicXml.book[2].excerpt, Is.EqualTo("Rose was remembering the illustrations from Morally Instructive Tales for the Nursery."));

        }

    }
}
