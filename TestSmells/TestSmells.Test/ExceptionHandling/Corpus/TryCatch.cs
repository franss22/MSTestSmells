using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Corpus
{
    [TestClass]
    public class UnitTest4321
    {

        [TestMethod]
        public void TestMethod1()
        {
            var a = new List<int>();
            try
            {
                a.Add(1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            var b = new List<int>();
            Assert.AreEqual(b, a);

        }
    }
}