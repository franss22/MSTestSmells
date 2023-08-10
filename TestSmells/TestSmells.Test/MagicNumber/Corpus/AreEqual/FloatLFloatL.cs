using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            
            Assert.AreEqual(1.5F, 2.5F, 0.2);
        }
    }
}