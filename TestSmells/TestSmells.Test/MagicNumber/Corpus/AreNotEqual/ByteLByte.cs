using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            byte b = 1;
            Assert.AreNotEqual((byte)1, b);
        }
    }
}