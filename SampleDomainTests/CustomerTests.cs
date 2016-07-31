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
    public class CustomerTests : ScopedTestClass
    {
        [TestMethod]
        public void Customer_CanBeSavedAndRetrieved()
        {
            var c = new Customer()
            {
                FirstName = "Bob",
                LastName = "Jones"
            };
            c.Save();

            var test = Customer.Get(c.CustomerId);

            Assert.AreEqual(c.CustomerId, test.CustomerId);
            Assert.AreEqual(c.FirstName, test.FirstName);
            Assert.AreEqual(c.LastName, test.LastName);
        }

        [TestMethod]
        public void Customer_LazyLoadsProducts()
        {
            var c = new Customer()
            {
                FirstName = "Bob",
                LastName = "Lazy Loads Products"
            };
            c.Save();

            var o = new SalesOrder()
            {
                Customer = c
            };
            o.Save();

            var p = new Product()
            {
                Name = "Customer_LazyLoadsProducts"
            };
            p.Save();

            var l = new LineItem()
            {
                Product = p,
                Quantity = 1,
                SalesOrder = o
            };
            l.Save();
            
            var test = Customer.Get(c.CustomerId);
            Assert.AreEqual(1, test.Products.Count);
            Assert.AreEqual(p.Name, test.Products[0].Name);
        }
    }
}
