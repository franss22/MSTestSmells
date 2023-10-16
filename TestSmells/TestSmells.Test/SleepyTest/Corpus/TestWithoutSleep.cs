using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;

namespace Corpus
{
    [TestClass]
    public class UnitTest432
    {

        [TestMethod]
        public void TestMethod1()
        {
            var a = new List<int>();
            var b = new List<int>();
            Assert.AreEqual(b, a);

        }
    }
}