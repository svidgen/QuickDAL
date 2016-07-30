using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleDomain;

namespace SampleDomainTests
{
    [TestClass]
    public class LineItemTests : ScopedTestClass
    {
        [TestMethod]
        public void LineItem_CanBeCreatedAndRetrievedById()
        {
            var line = new LineItem()
            {
                Quantity = 23
            };
            line.Save();

            var test = LineItem.Get(line.LineItemId);

            Assert.AreEqual(line.LineItemId, test.LineItemId);
            Assert.AreEqual(line.Quantity, test.Quantity);
        }

        [TestMethod]
        public void LineItem_LazyLoadsProduct()
        {
            var product = new Product() { Name = "whatever", UnitPrice = 22.22M };
            product.Save();
            var line = new LineItem()
            {
                Quantity = 23,
                Product = product
            };
            line.Save();

            var test = LineItem.Get(line.LineItemId);

            Assert.AreEqual(product.ProductId, test.Product.ProductId);
            Assert.AreEqual(product.Name, test.Product.Name);
        }

        [TestMethod]
        public void LineItem_LazyLoadsOrder()
        {
            var o = new SalesOrder();
            o.Save();

            var line = new LineItem()
            {
                SalesOrder = o
            };
            line.Save();

            var test = LineItem.Get(line.LineItemId);

            Assert.AreEqual(o.SalesOrderId, line.SalesOrder.SalesOrderId);
        }
    }
}
