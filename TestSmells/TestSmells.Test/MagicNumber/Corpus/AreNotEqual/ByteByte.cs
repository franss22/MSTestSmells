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
            byte b = 2;
            Assert.AreNotEqual(a, b);
        }
    }
}