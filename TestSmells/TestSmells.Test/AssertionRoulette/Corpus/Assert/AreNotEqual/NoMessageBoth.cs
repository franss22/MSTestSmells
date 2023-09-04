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
            Assert.AreNotEqual(a, b, 0.1);

            double c = 1d;
            double d = 2d;
            Assert.AreNotEqual(c, d, 0.1);
        }
    }
}