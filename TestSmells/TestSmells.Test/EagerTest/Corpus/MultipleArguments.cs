using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a = new Car();
            var b = new Car();


            Assert.AreEqual(a.Contains(2), a.Contains(1), "hello");
            Assert.AreEqual(a.Equals(b), b.Equals(a), "goobye");
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