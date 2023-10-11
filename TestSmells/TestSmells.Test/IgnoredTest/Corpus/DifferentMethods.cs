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

            Assert.AreNotEqual(b, a);
            b.Add(1);
            Assert.AreEqual(b, a);

        }
    }
}