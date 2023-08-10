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
            Assert.AreEqual(a, (nuint)2);
        }
    }
}