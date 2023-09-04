using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            string a = "abc";
            string b = "ab";
            StringAssert.EndsWith(a, b, "Expected a, actual b");

            string c = "cde";
            string d = "de";
            StringAssert.EndsWith(c, d, "Expected c, actual d");
        }
    }
}