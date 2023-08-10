using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            byte a = 1;
            Assert.AreEqual(a, (byte)2);
        }
    }
}