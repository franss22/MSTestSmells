using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            short a = 1;
            Assert.AreNotEqual(a, (short)2);
        }
    }
}