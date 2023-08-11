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
            Assert.AreEqual(a, b, false);

            string c = "foo";
            string d = "bar";
            Assert.AreEqual(c, d, false, "Expected c, actual d");
        }
    }
}