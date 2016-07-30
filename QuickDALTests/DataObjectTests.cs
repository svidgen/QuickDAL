using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using QuickDALTests.TestCaseClasses;

namespace QuickDALTests
{
    [TestClass]
    public class DataObjectTests : ScopedTestClass
    {

        public static void AssertFullMatch(SimpleDataObject a, SimpleDataObject b)
        {
            var dict_a = a.ToDictionary();
            var dict_b = b.ToDictionary();

            Assert.AreEqual(dict_a.Count, dict_b.Count);
            foreach (var k in dict_a.Keys)
            {
                Assert.AreEqual(dict_a[k], dict_b[k]);
            }
        }
        
        [TestMethod]
        public void DataObject_CanSerializeStrings()
        {
            var o = new SimpleDataObject()
            {
                StringValue = "a string"
            };
            var d = o.ToDictionary();
            Assert.AreEqual("a string", d["StringValue"]);
        }

        [TestMethod]
        public void DataObject_CanSerializeInt32s()
        {
            var o = new SimpleDataObject()
            {
                Int32Value = 32
            };
            var d = o.ToDictionary();
            Assert.AreEqual("32", d["Int32Value"]);
        }

        [TestMethod]
        public void DataObject_CanSerializeDoubles()
        {
            var o = new SimpleDataObject()
            {
                DoubleValue = 3.2
            };
            var d = o.ToDictionary();
            Assert.AreEqual("3.2", d["DoubleValue"]);
        }

        [TestMethod]
        public void DataObject_DoesNotSerializeBooleansByDefault()
        {
            var ot = new SimpleDataObject()
            {
                BooleanValue = true
            };
            var dt = ot.ToDictionary();
            Assert.IsFalse(dt.ContainsKey("BooleanValue"));
        }

        [TestMethod]
        public void DataObject_CanSerializeBooleans()
        {
            var ot = new SimpleDataObject()
            {
                BooleanValue = true
            };
            var dt = ot.ToDictionary(true);
            Assert.AreEqual("1", dt["BooleanValue"]);

            var of = new SimpleDataObject()
            {
                BooleanValue = false
            };
            var df = of.ToDictionary(true);
            Assert.AreEqual("0", df["BooleanValue"]);
        }

        [TestMethod]
        public void DataObject_CanSerializeGuids()
        {
            var guidstring = "CA37A611-B7F4-4EE8-AE2F-D32FB2D0D151";
            var guid = new Guid(guidstring);

            var o = new SimpleDataObject()
            {
                GuidValue = guid
            };
            var d = o.ToDictionary();

            Assert.AreEqual(guidstring.ToLower(), d["GuidValue"]);
        }

        [TestMethod]
        public void DataObject_CanBeSavedAndRetrievedById()
        {
            var guid = new Guid("AA37A611-B7F4-4EE8-AE2F-D32FB2D0D151");
            var distractorguid = new Guid("00000000-B7F4-4EE8-AE2F-D32FB2D0D151");

            var o = new SimpleDataObject()
            {
                BooleanValue = true,
                DoubleValue = 32,
                Int32Value = 64,
                GuidValue = guid,
                StringValue = "a string"
            };
            o.Save();

            var distractor = new SimpleDataObject()
            {
                GuidValue = distractorguid
            };
            distractor.Save();

            var test = SimpleDataObject.Get(guid);
            AssertFullMatch(o, test);
        }

        [TestMethod]
        public void DataObject_CanBeRetrievedByString()
        {
            var guid = new Guid("AB37A611-B7F4-4EE8-AE2F-D32FB2D0D151");
            var distractorguid = new Guid("00000000-B7F4-4EE8-AE2F-D32FB2D0D151");

            var str = "retrieval string";
            var o = new SimpleDataObject()
            {
                BooleanValue = true,
                DoubleValue = 12,
                Int32Value = 24,
                GuidValue = guid,
                StringValue = str
            };
            o.Save();

            var distractor = new SimpleDataObject()
            {
                GuidValue = distractorguid
            };
            distractor.Save();

            var test = SimpleDataObject.Get(new SimpleDataObject() { StringValue = str });
            Assert.AreEqual(1, test.Count);
            AssertFullMatch(o, test[0]);
        }

    }
}
