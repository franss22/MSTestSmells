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


            Assert.AreEqual(a.Contains(2), a.Contains(1), "hello");
            a.Equals(b);
            b.Equals(a);
        }

        [TestMethod]
        public void TestMethod1_2()
        {
            var a = new List<int>();
            var b = new List<int>();


            a.Contains(2);
            a.Contains(1);
            Assert.AreEqual(a.Equals(b), b.Equals(a), "goobye");
        }
    }
}