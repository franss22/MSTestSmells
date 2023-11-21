using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Corpus
{
    [TestClass]
    public class UnitTest4023
    {

        private void MyTestFunction(object a,  object b)
        {
            return;
        }



        [TestMethod]
        public void TestMethod1()
        {
            var a = new List<int>();
            var b = new List<int>();

            MyTestFunction(b, a);

        }
    }
}