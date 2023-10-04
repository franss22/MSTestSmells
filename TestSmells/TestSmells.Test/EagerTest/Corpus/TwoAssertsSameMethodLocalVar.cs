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

            var ex1 = a.Equals(b);
            var ex2 = b.Equals(a);
            
            Assert.IsTrue(ex1);
            Assert.IsTrue(ex2);
        }
    }
}