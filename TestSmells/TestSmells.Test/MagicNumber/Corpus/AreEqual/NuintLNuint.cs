using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            nuint b = 1;
            Assert.AreEqual((nuint)1, b);
        }
    }
}