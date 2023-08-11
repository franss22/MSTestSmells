using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            bool a = true;
            Assert.IsFalse(a, "Expected a, actual b");

            bool c = true;
            Assert.IsFalse(c, "Expected c, actual d");
        }
    }
}