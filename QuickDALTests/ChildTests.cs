using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickDALTests.TestCaseClasses;

namespace QuickDALTests
{
    [TestClass]
    public class ChildTests : ScopedTestClass
    {
        [TestMethod]
        public void Child_CanBeCreatedAndRetrievedById()
        {
            var c = new Child()
            {
                Name = "child a"
            };
            c.Save();

            var test = Child.Get(c.ChildId);

            Assert.AreEqual(c.ChildId, test.ChildId);
            Assert.AreEqual(c.Name, test.Name);
        }

        [TestMethod]
        public void Child_LazyLoadsParent()
        {
            var Parent = new Parent() { Name = "whatever" };
            Parent.Save();
            var line = new Child()
            {
                Parent = Parent
            };
            line.Save();

            var test = Child.Get(line.ChildId);

            Assert.AreEqual(Parent.ParentId, test.Parent.ParentId);
            Assert.AreEqual(Parent.Name, test.Parent.Name);
        }

    }
}
