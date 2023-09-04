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
            File.AppendAllText(path, "foo");
            var data = File.ReadAllLines(path);

            Assert.IsNotNull(data);
        }
    }
}