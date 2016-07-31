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
            var c = CreateCustomerWithProduct("Customer_LazyLoadsProducts");

            var test = Customer.Get(c.CustomerId);

            Assert.AreEqual(1, test.Products.Count);
            Assert.IsFalse(String.IsNullOrEmpty(c.Products[0].Name));
            Assert.AreEqual(c.Products[0].Name, test.Products[0].Name);
        }

        [TestMethod]
        public void Customer_DoesNotLazyLoadOtherCustomerProducts()
        {
            var c1 = CreateCustomerWithProduct("Customer_DoesNotLazyLoadOtherCustomerProducts1");
            var c2 = CreateCustomerWithProduct("Customer_DoesNotLazyLoadOtherCustomerProducts2");

            var test1 = Customer.Get(c1.CustomerId);
            var test2 = Customer.Get(c2.CustomerId);

            Assert.AreEqual(1, test1.Products.Count);
            Assert.AreEqual(1, test2.Products.Count);
            Assert.AreEqual(c1.Products[0].Name, test1.Products[0].Name);
            Assert.AreEqual(c2.Products[0].Name, test2.Products[0].Name);
            Assert.AreNotEqual(test1.Products[0].Name, test2.Products[0].Name);
        }

        public static Customer CreateCustomerWithProduct(String testName)
        {
            var c = new Customer()
            {
                FirstName = "Bob",
                LastName = testName
            };
            c.Save();

            var o = new SalesOrder()
            {
                Customer = c
            };
            o.Save();

            var p = new Product()
            {
                Name = testName + " Product"
            };
            p.Save();

            var l = new LineItem()
            {
                Product = p,
                Quantity = 1,
                SalesOrder = o
            };
            l.Save();

            return c;
        }

    }
}
