using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            sbyte b = 1;
            Assert.AreEqual((sbyte)1, b);
        }
    }
}