using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            ulong b = 1;
            Assert.AreNotEqual(1ul, b);
        }
    }
}