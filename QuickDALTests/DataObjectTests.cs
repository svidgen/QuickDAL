﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickDALTests.SampleClasses;

namespace QuickDALTests
{
    [TestClass]
    public class DataObjectTests
    {
        
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


    }
}
