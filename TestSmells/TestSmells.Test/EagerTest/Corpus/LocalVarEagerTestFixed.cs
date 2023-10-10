using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a = new List<int>();
            var b = new List<int>();

            var ex1 = a.Contains(1);
            var ex2 = a.Equals(b);

            Assert.IsTrue(ex1);
        }

        [TestMethod]
        public void TestMethod1_1()
        {
            var a = new List<int>();
            var b = new List<int>();

            var ex1 = a.Contains(1);
            var ex2 = a.Equals(b);

            Assert.IsTrue(ex2);
        }
    }
}