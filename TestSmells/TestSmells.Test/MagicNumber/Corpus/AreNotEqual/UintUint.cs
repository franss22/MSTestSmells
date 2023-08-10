using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            uint a = 1;
            uint b = 2;
            Assert.AreNotEqual(a, b);
        }
    }
}