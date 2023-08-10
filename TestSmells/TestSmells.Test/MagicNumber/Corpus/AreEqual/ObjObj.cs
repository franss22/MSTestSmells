using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{

    public class Car
    {
        string Name;
        public Car(string name)
        {
            Name = name;
        }

    }

    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            Car a = new Car("a");
            Car b = new Car("b");
            Assert.AreEqual(a, b);
        }
    }
}