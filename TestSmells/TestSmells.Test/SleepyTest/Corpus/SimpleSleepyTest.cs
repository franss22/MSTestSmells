using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;

namespace Corpus
{
    [TestClass]
    public class UnitTest4
    {

        [TestMethod]
        public void TestMethod1()
        {
            var a = new List<int>();
            var b = new List<int>();
            Thread.Sleep(1000);
            Assert.AreEqual(b, a);

        }
    }
}