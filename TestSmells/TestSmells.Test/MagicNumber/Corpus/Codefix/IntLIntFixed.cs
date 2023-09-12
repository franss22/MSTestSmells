using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            int b = 1;
            const int expected = 1;
            Assert.AreEqual(expected, b);
        }
    }
}