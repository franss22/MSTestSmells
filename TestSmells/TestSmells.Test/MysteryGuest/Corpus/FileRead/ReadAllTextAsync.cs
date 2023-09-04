using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var data = await File.ReadAllTextAsync(path);

            Assert.AreEqual(data, "1324");
        }
    }
}