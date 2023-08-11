using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            object a = 1;
            object b = 2;
            Assert.AreNotSame(a, b);

            object c = 1;
            object d = 2;
            Assert.AreNotSame(c, d);
        }
    }
}