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
            Assert.AreNotEqual(a, b, 0.1, "Expected a, actual b");
        }
    }
}