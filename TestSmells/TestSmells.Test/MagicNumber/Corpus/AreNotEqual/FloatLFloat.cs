using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            float b = 2.5F;
            Assert.AreNotEqual(1.5F, b, 0.2);
        }
    }
}