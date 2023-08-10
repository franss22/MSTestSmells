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
            Assert.AreNotEqual(a, (ushort)2);
        }
    }
}