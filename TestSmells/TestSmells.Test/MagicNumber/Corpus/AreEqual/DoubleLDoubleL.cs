using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            
            Assert.AreEqual(1.5d, 2.5d, 0.2);
        }
    }
}