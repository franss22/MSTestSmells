using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            double a = 1.5;
            double b = 2.5;
            Assert.AreEqual(a, b, 0.2);
        }
    }
}