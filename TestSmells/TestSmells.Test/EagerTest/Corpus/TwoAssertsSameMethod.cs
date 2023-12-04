using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a = new Car();
            var b = new Car();

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));
        }
    }
    public class Car
    {
        public Car() { }
        public bool Contains(int a)
        {
            return a == 1;
        }
        public bool Equals(Car other)
        {
            return other == this;
        }
    }
}