using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSmells.Test.ObviousFail.Corpus
{
    [TestClass]
    internal class isFalseTrue
    {
        [TestMethod]
        public void TestMethod2()
        {
            Assert.Fail();
        }
    }
}
