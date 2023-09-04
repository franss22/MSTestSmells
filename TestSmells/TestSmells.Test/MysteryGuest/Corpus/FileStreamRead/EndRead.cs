using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var path = @"C:\Program Files\AMD\atikmdag_dce.log";
            var file = new FileStream(path, FileMode.Open);

            var data = file.EndRead(CallEnd());

            Assert.IsNotNull(data);
        }

        public IAsyncResult CallEnd()
        {
            throw new NotImplementedException();
        }
    }
}