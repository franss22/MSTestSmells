using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            
            Assert.AreNotEqual(1.5d, 2.5d, 0.2);
        }
    }
}