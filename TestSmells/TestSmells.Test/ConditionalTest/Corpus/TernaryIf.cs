using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest3
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a = new List<int>();
            var b = new List<int>();
            b[0] = a.Count > 1 ? 1 : 0;

            Assert.AreEqual(b, a);

        }
    }
}