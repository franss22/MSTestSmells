using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            CultureInfo myCIintl = new CultureInfo("es-ES", false);
            string a = "foo";
            string b = "bar";
            Assert.AreNotEqual(a, b, false, myCIintl, "Expected a, actual b");
        }
    }
}