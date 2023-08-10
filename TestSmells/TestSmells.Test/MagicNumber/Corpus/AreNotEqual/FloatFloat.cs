using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            float a = 1.5F;
            float b = 2.5F;
            Assert.AreNotEqual(a, b, 0.2);
        }
    }
}