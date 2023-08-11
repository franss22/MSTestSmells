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
            Assert.IsNotNull(a, "Expected a, actual b");

            object c = 1;
            Assert.IsNotNull(c, "Expected c, actual d");
        }
    }
}