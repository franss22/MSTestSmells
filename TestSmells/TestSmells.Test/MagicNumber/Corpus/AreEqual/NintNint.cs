using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            nint a = 1;
            nint b = 2;
            Assert.AreEqual(a, b);
        }
    }
}