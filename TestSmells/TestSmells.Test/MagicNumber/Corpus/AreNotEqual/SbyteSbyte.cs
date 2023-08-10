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
            sbyte b = 2;
            Assert.AreNotEqual(a, b);
        }
    }
}