using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace DynamicXml.UnitTests
{
    /// <summary>
    /// Contains UnitTests for accessing XElement's underlying value
    /// </summary>
    public partial class DynamicXElementTest 
    {
        //---------------------------------------------------------------------------------------//
        // Helper methods
        //---------------------------------------------------------------------------------------//
        /// <summary>
        /// Generic helper method for testing value access.
        /// This method creates XElement and add <paramref name="value"/> as is without
        /// any casts or without calling any methods.
        /// </summary>
        private static void GenericValueAccessTestImpl<T>(T value)
        {
            XElement element = new XElement("name", value);
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            T actual = dynamicElement;
            Assert.That(actual, Is.EqualTo(value));

            Console.WriteLine("Actual string value: {0}", actual);
        }

        /// <summary>
        /// Another generic helper method for testing access to underlying XElement's value.
        /// But this method calls "ToString" method on <paramref name="value"/> before
        /// passing this value to XElement's constructors.
        /// </summary>
        private static void GenericValueAccessWithToStringImpl<T>(T value)
        {
            XElement element = new XElement("name", value.ToString());
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            T actual = dynamicElement;
            Assert.That(actual, Is.EqualTo(value));

            Console.WriteLine("Actual string value: {0}", actual);
        }

        //---------------------------------------------------------------------------------------//
        // Accessing value
        //---------------------------------------------------------------------------------------//
        [TestCase]
        public void AccessMissedElementTest()
        {
            XElement element = new XElement("name");
            dynamic dynamicElement = element.AsDynamic();
            try
            {
                string value = dynamicElement.Command.SubCommand.Value;
            }
            catch
            {
                Console.WriteLine(element);
                Assert.Throws(typeof(Exception), () => { XElement e = dynamicElement.Command; });
            }
        }

        [TestCase]
        public void AccessXElementTest()
        {
            // Cast to XElement should lead to obtaining underlying XElement
            XElement element = new XElement("name", "value");
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            XElement underlyingElement = dynamicElement;
            Assert.That(object.ReferenceEquals(underlyingElement, element));
        }

        
        [TestCase]
        public void AccessDateTimeValueTest()
        {
            GenericValueAccessTestImpl(DateTime.Now);
        }

        [TestCase]
        public void AccessTimeSpanValueTest()
        {
            // We should use GenericValueAccessWithToStringImpl method
            // because we should call ToString on TimSpan argument
            // before passing it to XElement constructor.
            GenericValueAccessWithToStringImpl(TimeSpan.FromSeconds(123123));
        }


        [TestCase]
        public void AccessStringValueElementTest()
        {
            GenericValueAccessTestImpl("string value");
        }

        [TestCase]
        public void AccessIntValueElementTest()
        {
            GenericValueAccessTestImpl(12);
        }

        [TestCase]
        public void AccessDoubleValueElementTest()
        {
            GenericValueAccessTestImpl(12f);
        }

        //---------------------------------------------------------------------------------------//
        // Calling Binder.Convert method
        //---------------------------------------------------------------------------------------//

        // For all implicit nad implicit casts with dynamic objects C# compiler generates
        // appropriate calls to Binder.Convert method wrapped into CallSite<>.
        // This call leads to calling TryConvert override method defined in DynamicXml class.
        // This approach is key feature for using parametrized unit tests.

        /// <summary>
        /// This is simple method that uses Binder.Convert method for converting underlying
        /// XElement value.
        /// This method helps upderstand following generic method that uses parametrized unit
        /// test.
        /// </summary>
        [TestCase]
        public void AccessValueWithBinderTest()
        {
            XElement element = new XElement("name", 1);
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            // Following lines are almost equivalent (except caching)
            // to following:
            // int actual = (int)dynamicElement;

            var callsiteBinder = Binder.Convert(CSharpBinderFlags.None, typeof(int), 
                                                    typeof(DynamicXElement));
            
            CallSite<Func<CallSite, object, int>> callSite =
                 CallSite<Func<CallSite, object, int>>.Create(callsiteBinder);

            int actual = callSite.Target(callSite, dynamicElement);
            Assert.That(actual, Is.EqualTo(1));
        }

        /// <summary>
        /// Generic parametrized unit test for testing extrancting XElement values
        /// with different input data.
        /// </summary>
        /// <example>
        /// [TestCase("name", 1, typeof(int), Result = 1)]
        /// Means that we passing elenetName = "name" and value = 1 and expecting the result
        /// of type int with value equals to 1.
        /// </example>
        /// <remarks>
        /// See <code>AccessValueWithBinderTest</code> for more information about compiler behavior.
        /// </remarks>
        [TestCase("name", 1, typeof(int), Result = 1)]
        [TestCase("name", 1.1, typeof(double), Result = 1.1)]
        [TestCase("name", "value", typeof(string), Result = "value")]
        [TestCase("name", int.MinValue, typeof(int), Result = int.MinValue)]
        [TestCase("name", int.MaxValue, typeof(int), Result = int.MaxValue)]
        [TestCase("name", long.MinValue, typeof(long), Result = long.MinValue)]
        [TestCase("name", long.MaxValue, typeof(long), Result = long.MaxValue)]
        [TestCase("name", uint.MinValue, typeof(uint), Result = uint.MinValue)]
        [TestCase("name", uint.MaxValue, typeof(uint), Result = uint.MaxValue)]
        [TestCase("name", byte.MinValue, typeof(byte), Result = byte.MinValue)]
        [TestCase("name", byte.MaxValue, typeof(byte), Result = byte.MaxValue)]
        public object AccessValueElementTest(string elementName, object value, Type type)
        {
            XElement element = new XElement(elementName, value);
            dynamic dynamicElement = DynamicXElement.CreateInstance(element);
            CallSiteBinder callsiteBinder = Binder.Convert(CSharpBinderFlags.None, type, 
                                                           typeof(DynamicXElement));

            //-----------------------------------------------------------------------------------//
            // Creating Func<CallSite, object, Type> 
            //-----------------------------------------------------------------------------------//
            // For type "int" this code is equivalent to: 
            // Type constructedFuncType = typeof(Func<CallSite, object, int);
            
            Type openFuncType = typeof(Func<,,>);
            Type constructedFuncType = openFuncType.MakeGenericType(
                typeof(CallSite), typeof(object), type);

            //-----------------------------------------------------------------------------------//
            // Creating CallSite<Func<CallSite, object, Type>>
            //-----------------------------------------------------------------------------------//
            // For type "int" this code is equivalent to:
            // Type constructedCallSiteType = typeof(CallSite<Func<CallSite, object, int>>);
            Type callSiteType = typeof(CallSite<>);
            Type constructedCallSiteType = callSiteType.MakeGenericType(constructedFuncType);

            //-----------------------------------------------------------------------------------//
            // var callSite = CallSite<Func<CallSite, object, int>>.Create(callSiteBinder);
            //-----------------------------------------------------------------------------------//
            var method = constructedCallSiteType.GetMethod("Create", 
                                BindingFlags.Static | BindingFlags.Public);
            CallSite callSite = (CallSite)method.Invoke(null, new object[] { callsiteBinder });

            //-----------------------------------------------------------------------------------//
            // int actual = callSite.Target(callSite, dynamicElement);
            //-----------------------------------------------------------------------------------//
            FieldInfo targetFieldInfo = callSite.GetType().GetField("Target");
            object target = targetFieldInfo.GetValue(callSite);
            
            // target is underlying delegate instance of type: Func<CallSite, object, int>
            var targetMethod = target.GetType().GetMethod("Invoke", 
                                    BindingFlags.Instance | BindingFlags.Public);
            object actual = targetMethod.Invoke(target, new object[] { callSite, dynamicElement });

            Console.WriteLine("Name: {0}, Value: {1}, Type: {2}", elementName, value, type);
            Console.WriteLine("Actual value: {0}", actual);
            return actual;
        }

        //---------------------------------------------------------------------------------------//
        // Accessing values by index
        //---------------------------------------------------------------------------------------//
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
            // Precondition should failed accessing invalid subelements index
            string value = dynamicElement[1];
            Assert.Fail();
        }
    }
}