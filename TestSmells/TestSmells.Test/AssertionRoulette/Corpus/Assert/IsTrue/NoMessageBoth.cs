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
            Assert.IsTrue(a);

            bool c = true;
            Assert.IsTrue(c);
        }
    }
}