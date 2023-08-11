using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            int a = 1;
            int b = 2;
            Assert.AreEqual(a, b, "Expected a, actual b");
        }
    }
}