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
            Assert.IsNotInstanceOfType(a, typeof(int), "Expected a, actual typeof(int)");

            object c = 1;
            Assert.IsNotInstanceOfType(c, typeof(int), "Expected c, actual typeof(int)");
        }
    }
}