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
            for (int i = 0; i < a.Count; i++)
            {
                Assert.AreEqual(b, a);
            }
        }
    }
}