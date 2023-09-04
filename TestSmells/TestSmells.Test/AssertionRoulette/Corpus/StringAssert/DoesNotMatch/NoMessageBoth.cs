using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
namespace Corpus
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            Regex rx = new Regex(@"\b(?<word>\w+)\s+(\k<word>)\b",
          RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string a = "abc";
            StringAssert.DoesNotMatch(a, rx);

            string c = "cde";
            StringAssert.DoesNotMatch(c, rx);
        }
    }
}