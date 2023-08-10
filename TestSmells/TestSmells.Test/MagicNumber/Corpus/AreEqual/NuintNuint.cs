using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            nuint a = 1;
            nuint b = 2;
            Assert.AreEqual(a, b);
        }
    }
}