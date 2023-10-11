using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1_1()
        {
            var a = new List<int>();
            var b = new List<int>();

            Assert.IsTrue(a.Contains(1));
            a.Equals(b);
        }

        [TestMethod]
        public void TestMethod1_2()
        {
            var a = new List<int>();
            var b = new List<int>();

            a.Contains(1);
            Assert.IsTrue(a.Equals(b));
        }
    }
}