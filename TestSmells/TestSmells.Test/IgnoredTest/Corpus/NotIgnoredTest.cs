using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest5
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