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
            Assert.AreEqual(a, 2.5d, 0.2);
        }
    }
}