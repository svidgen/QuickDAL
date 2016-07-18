using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using System.Diagnostics;

namespace QuickDALTests
{
    [TestClass()]
    public class TestEnvironment
    {

        private static TestContext CurrentTestContext;
        public static Random RNG = new Random();

        [AssemblyInitialize]
        public static void InitializeEnvironment(TestContext context)
        {
            // Locator.UseTestLocator();
            CurrentTestContext = context;
            Debug.WriteLine("Starting " + context.TestName + " ...");
        }

        [AssemblyCleanup]
        public static void DestoryEnvironment()
        {
            Debug.WriteLine("All cleaned up!");
        }

    }

    [TestClass]
    public class ScopedTestClass : IDisposable
    {

        private TransactionScope TxnScope;

        public ScopedTestClass()
        {
            TxnScope = new TransactionScope(TransactionScopeOption.RequiresNew);
        }

        public void Dispose()
        {
            TxnScope.Dispose();
        }

        [TestInitialize]
        public void PrepareForTest()
        {
            try
            {
                // nothing to prepare right now.
            }
            catch
            {
                Debug.Assert(false, "Test cleanup logout failed");
            }

            // clear any cached items between tests
            // nothing to do here yet.
        }

        [TestCleanup]
        public void CleanupAfterTest()
        {
            // Debug.WriteLine("Cleaning up after " + TestContext.TestName);
            // Domain.ApplicationUser.Logout();
            // TxnScope.Dispose();
            // System.Threading.Monitor.Exit(ScopedTestClass.testclasslock);
        }

    }
}
