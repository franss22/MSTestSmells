using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            ulong a = 1;
            ulong b = 2;
            Assert.AreEqual(a, b);
        }
    }
}