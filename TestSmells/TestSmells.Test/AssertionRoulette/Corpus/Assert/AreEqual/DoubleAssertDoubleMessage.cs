using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            double a = 1d;
            double b = 2d;
            Assert.AreEqual(a, b, 0.1, "Expected a, actual b");

            double c = 1d;
            double d = 2d;
            Assert.AreEqual(c, d, 0.1, "Expected c, actual d");
        }
    }
}