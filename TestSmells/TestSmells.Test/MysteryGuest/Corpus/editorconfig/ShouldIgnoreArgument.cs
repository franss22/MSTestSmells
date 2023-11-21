using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TestProject1
{
    [TestClass]
    internal class MysteryGuest
    {
        [TestMethod]
        public void TestMethod()
        {
            var data = File.ReadAllLines(@"C:\Program Files\AMD\atikmdag_dce.log");

            Assert.IsNotNull(data);
        }
    }
}