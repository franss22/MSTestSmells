using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            long a = 1;
            long b = 2;
            Assert.AreEqual(a, b);
        }
    }
}