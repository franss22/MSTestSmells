using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            float a = 1f;
            float b = 2f;
            Assert.AreNotEqual(a, b, 0.1, "Expected a, actual b");
        }
    }
}