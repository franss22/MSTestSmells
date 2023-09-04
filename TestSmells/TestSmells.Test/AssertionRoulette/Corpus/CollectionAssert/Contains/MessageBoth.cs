using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            int[] a = {1, 2, 3};
            int b = 2;
            CollectionAssert.Contains(a, b, "Expected a, actual b");

            int[] c = {1, 2, 3};
            int d = 4;
            CollectionAssert.Contains(c, d, "Expected c, actual d");
        }
    }
}