using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            
            Assert.AreNotEqual((nint)1, (nint)2);
        }
    }
}