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
            var arr = new byte[file.Length];
            var callback = new AsyncCallback(Call);
            var data = file.BeginWrite(arr, 2, 1, callback, arr);

            Assert.IsNotNull(data);
        }

        public void Call(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }
    }
}