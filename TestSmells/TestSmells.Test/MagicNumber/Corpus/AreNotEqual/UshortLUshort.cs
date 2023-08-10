using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            ushort b = 1;
            Assert.AreNotEqual((ushort)1, b);
        }
    }
}