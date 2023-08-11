using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            string a = "foo";
            string b = "bar";
            Assert.AreEqual(a, b, false, "Expected a, actual b");
        }
    }
}