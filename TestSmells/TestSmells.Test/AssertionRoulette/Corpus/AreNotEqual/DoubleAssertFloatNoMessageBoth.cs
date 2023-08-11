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
            Assert.AreNotEqual(a, b, 0.1);

            float c = 1f;
            float d = 2f;
            Assert.AreNotEqual(c, d, 0.1);
        }
    }
}