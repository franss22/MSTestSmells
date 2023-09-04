using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var a = new List<int>();
            Assert.ThrowsException<ArgumentNullException>(() => a.Add(1), "Exception");

            var c = new List<int>();
            Assert.ThrowsException<ArgumentNullException>(() => c.Add(1), "Exception");
        }
    }
}