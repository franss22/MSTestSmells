using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            string text = "foo";
            File.WriteAllText(path, text);
            var data = File.ReadAllBytes(path);
            Assert.IsNotNull(data);
        }
    }
}