using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            float a = 1.5F;
            Assert.AreNotEqual(a, 2.5F, 0.2);
        }
    }
}