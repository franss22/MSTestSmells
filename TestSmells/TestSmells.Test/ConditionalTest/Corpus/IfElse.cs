using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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
            if (a.Count > 1)
            {
                Assert.AreEqual(b, a);
            }
            else
            {
                Assert.AreNotEqual(b, a);
            }
        }
    }
}