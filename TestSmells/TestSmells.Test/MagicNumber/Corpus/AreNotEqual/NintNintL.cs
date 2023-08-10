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
            Assert.AreNotEqual(a, (nint)2);
        }
    }
}