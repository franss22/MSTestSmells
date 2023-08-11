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
            Assert.AreEqual(a, b, false, myCIintl);
        }
    }
}