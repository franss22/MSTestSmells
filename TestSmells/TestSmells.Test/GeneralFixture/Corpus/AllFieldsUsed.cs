using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        int field_num1;
        int field_num2;
        int field_num3 = 2;


        [TestInitialize]
        public void init()
        {
            field_num1 = 1;
            field_num2 = 2;
            field_num3 = 3;
        }

        [TestMethod]
        public void TestMethod()
        {
            int a = field_num1+field_num2+field_num3;
            const int b = 2;
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void TestMethod1()
        {
            int a = field_num2*2;
            Assert.AreEqual(a, field_num1+field_num3);
        }
    }
}