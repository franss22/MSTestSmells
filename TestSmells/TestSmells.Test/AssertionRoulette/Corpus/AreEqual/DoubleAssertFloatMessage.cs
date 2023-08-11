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
            Assert.AreEqual(a, b, 0.1, "Expected a, actual b");

            float c = 1f;
            float d = 2f;
            Assert.AreEqual(c, d, 0.1, "Expected c, actual d");
        }
    }
}