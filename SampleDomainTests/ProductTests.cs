﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleDomain;

namespace SampleDomainTests
{
    [TestClass]
    public class ProductTests : ScopedTestClass
    {
        [TestMethod]
        public void Product_CanBeSavedAndRetrievedById()
        {
            var p = new Product()
            {
                Name = "product a",
                UnitPrice = 12.23M
            };
            p.Save();

            var test = Product.Get(p.ProductId);

            Assert.AreEqual(p.ProductId, test.ProductId);
            Assert.AreEqual(p.Name, test.Name);
            Assert.AreEqual(p.UnitPrice, test.UnitPrice);
        }
    }
}
