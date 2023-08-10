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
            Assert.AreEqual(a, b);

            int c = 1;
            int d = 2;
            Assert.AreEqual(c, d, "Expected c, actual d");
        }
    }
}