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
            Assert.IsFalse(a);

            bool c = true;
            Assert.IsFalse(c);
        }
    }
}