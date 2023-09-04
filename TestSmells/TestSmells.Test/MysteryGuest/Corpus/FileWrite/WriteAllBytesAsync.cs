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
            byte[] bytes = { 1, 1, 1, 2, 3 };
            await File.WriteAllBytesAsync(path, bytes);
            var data = File.ReadAllBytes(path);
            Assert.IsNotNull(data);
        }
    }
}