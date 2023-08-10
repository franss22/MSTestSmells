using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            nuint a = 1;
            Assert.AreNotEqual(a, (nuint)2);
        }
    }
}