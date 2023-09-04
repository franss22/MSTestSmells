using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethodAssertionRoulette()
        {
            const int a = 1;
            const int b = 2;
            const int c = 3;

            Assert.AreEqual(a, b, "foo {0}", a);
            Assert.AreEqual(b, c);
        }
    }
}