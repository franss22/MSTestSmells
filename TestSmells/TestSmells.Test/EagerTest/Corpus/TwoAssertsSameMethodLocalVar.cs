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

            var ex1 = a.Equals(b);
            var ex2 = b.Equals(a);
            
            Assert.IsTrue(ex1);
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