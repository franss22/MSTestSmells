using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a = new List<int>();
            var b = new List<int>();

            switch (a[0])
            {
                case 1:
                    Assert.AreEqual(b, a);
                    break;
                case 2:
                    Assert.AreNotEqual(b, a);
                    break;
                default:
                    Assert.Fail();
                    break;
            }

        }
    }
}