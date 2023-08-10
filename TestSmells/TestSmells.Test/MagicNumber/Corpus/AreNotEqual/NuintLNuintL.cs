using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            
            Assert.AreNotEqual((nuint)1, (nuint)2);
        }
    }
}