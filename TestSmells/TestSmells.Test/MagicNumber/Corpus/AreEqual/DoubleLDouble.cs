using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            double b = 2.5;
            Assert.AreEqual(1.5d, b, 0.2);
        }
    }
}