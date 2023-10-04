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

            var expected = a.Equals(b);
            var expected2 = a.Contains(1);
            Assert.IsTrue(expected2);
        }
    }
}