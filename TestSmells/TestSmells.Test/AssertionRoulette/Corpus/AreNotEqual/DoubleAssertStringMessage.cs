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
            Assert.AreNotEqual(a, b, false, "Expected a, actual b");

            string c = "foo";
            string d = "bar";
            Assert.AreNotEqual(c, d, false, "Expected c, actual d");
        }
    }
}