using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            ushort a = 1;
            ushort b = 2;
            Assert.AreEqual(a, b);
        }
    }
}