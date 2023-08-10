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
            Assert.AreEqual(a, (ushort)2);
        }
    }
}