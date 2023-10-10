using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest3
    {
        public int dead = 1;

        [TestMethod]
        public void TestMethod1()
        {
            var a = 1;
            var b = 1;

            Assert.AreEqual(b, a);
        }
    }
}