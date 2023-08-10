using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            sbyte a = 1;
            Assert.AreEqual(a, (sbyte)2);
        }
    }
}