using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async void TestMethod()
        {
            var path = @"C:\Program Files\AMD\atikmdag_dce.log";
            var file = new FileStream(path, FileMode.Open);
            var arr = new byte[file.Length];
            var data = await file.ReadAsync(arr, 1, 2);//, new System.Threading.CancellationToken());

            Assert.IsNotNull(data);
        }

    }
}