using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            ulong a = 1;
            Assert.AreNotEqual(a, 2ul);
        }
    }
}