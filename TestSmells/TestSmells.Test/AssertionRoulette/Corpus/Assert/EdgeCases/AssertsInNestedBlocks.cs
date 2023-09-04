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
            if (a != b)
            {
                Assert.AreEqual(a, b, 0.1);
            }

            double c = 1d;
            double d = 2d;
            if (c != b)
            {
                Assert.AreEqual(c, d, 0.1);
            }
        }
    }
}