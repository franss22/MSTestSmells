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
            Assert.AreSame(a, b, "Expected a, actual b");
        }
    }
}