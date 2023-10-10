using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a = new List<int>();
            var b = new List<int>();
            foreach (int i in a)
            {
                Assert.AreEqual(b, a);
            }
        }
    }
}