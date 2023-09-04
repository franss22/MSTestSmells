using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        public static async Task asyncMethod()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine(" Method 1");
                    // Do something
                    Task.Delay(100).Wait();
                }
            });
        }
        [TestMethod]
        public void TestMethod()
        {
            var a = new List<int>();
            Assert.ThrowsExceptionAsync<ArgumentNullException>(asyncMethod);

            var c = new List<int>();
            Assert.ThrowsExceptionAsync<ArgumentNullException>(asyncMethod);
        }
    }
}