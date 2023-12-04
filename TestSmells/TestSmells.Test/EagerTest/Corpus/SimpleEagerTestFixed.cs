using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Corpus
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1_1()
        {
            var a = new Car();
            var b = new Car();

            Assert.IsTrue(a.Contains(1));
            a.Equals(b);
        }

        [TestMethod]
        public void TestMethod1_2()
        {
            var a = new Car();
            var b = new Car();

            a.Contains(1);
            Assert.IsTrue(a.Equals(b));
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