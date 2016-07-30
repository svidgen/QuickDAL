using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickDALTests.TestCaseClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickDALTests
{
    [TestClass]
    public class ParentTests : ScopedTestClass
    {
        [TestMethod]
        public void Parent_CanBeSavedAndRetrievedById()
        {
            var p = new Parent()
            {
                Name = "parent a"
            };
            p.Save();

            var test = Parent.Get(p.ParentId);

            Assert.AreEqual(p.ParentId, test.ParentId);
            Assert.AreEqual(p.Name, test.Name);
        }

        [TestMethod]
        public void Parent_ChildrenLazyLoad()
        {
            var p = new Parent()
            {
                Name = "Parent_ChildrenLazyLoad"
            };
            p.Save();

            for (var i = 0; i < 3; i++)
            {
                var c = new Child()
                {
                    Parent = p,
                    Name = "Parent_ChildrenLazyLoad Child " + i
                };
                c.Save();
            }

            var test = Parent.Get(p.ParentId);

            Assert.AreEqual(3, p.Children.Count);
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual("Parent_ChildrenLazyLoad Child " + i, p.Children[i].Name);
            }
        }
    }
}
