using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoUT.Tests
{
    [TestClass]
    [TestCategory("Class 1")]
    public class UTTextTests
    {
        [TestMethod]
        [TestCategory("Class 1 Test Concat")]
        public void TestConcat()
        {
            Assert.AreEqual("ab", UTText.Concat("a", "b"));
        }
    }
}
