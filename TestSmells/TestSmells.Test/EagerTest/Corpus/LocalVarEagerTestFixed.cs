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

            var ex1 = a.Contains(1);
            var ex2 = a.Equals(b);

            Assert.IsTrue(ex1);

        }

        [TestMethod]
        public void TestMethod1_2()
        {
            var a = new Car();
            var b = new Car();

            var ex1 = a.Contains(1);
            var ex2 = a.Equals(b);


            Assert.IsTrue(ex2);
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