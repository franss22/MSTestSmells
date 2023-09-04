using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async void TestMethod()
        {
            var path = @"C:\Program Files\AMD\atikmdag_dce.log";
            await foreach (var data in  File.ReadLinesAsync(path))
                {Assert.IsNotNull(data);}

            
        }
    }
}