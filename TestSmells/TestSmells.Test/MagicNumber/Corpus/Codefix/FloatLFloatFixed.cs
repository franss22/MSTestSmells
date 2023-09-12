using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            float b = 1f;
            const float expected = 1f;
            Assert.AreEqual(expected, b);
        }
    }
}