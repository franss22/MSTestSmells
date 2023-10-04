
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
    TestSmells.MagicNumber.MagicNumberAnalyzer,
    TestSmells.MagicNumber.MagicNumberCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.MagicNumber
{
    [TestClass]
    public class MagicNumberCodefixUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("MagicNumber", "Corpus", "Codefix");
        /* 
         * Tests are named by the type of value given to the assertion method
         * L means the value is a literal
         * For example, IntLInt means the assertion method is given an integer literal value, 
         * then an integer value (in a variable)
         * 
         * Types that don't have a literal suffix are cast inside the method (Ex: Assert.AreEqual((byte)1, b)
         */



        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task CodefixIntLInt()
        {
            var testFile = @"IntLInt.cs";
            var fixedFile = @"IntLIntFixed.cs";

            var expected = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 30).WithArguments("AreEqual", "1");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task CodefixIntIntL()
        {
            var testFile = @"IntIntL.cs";
            var fixedFile = @"IntIntLFixed.cs";

            var expected = VerifyCS.Diagnostic().WithSpan(12, 32, 12, 33).WithArguments("AreEqual", "2");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task CodefixFloatLFloat()
        {
            var testFile = @"FloatLFloat.cs";
            var fixedFile = @"FloatLFloatFixed.cs";

            var expected = VerifyCS.Diagnostic().WithSpan(12, 29, 12, 31).WithArguments("AreEqual", "1f");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task CodefixIntLCastInt()
        {
            var testFile = @"IntLCastInt.cs";
            var fixedFile = @"IntLCastIntFixed.cs";

            var expected = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 35).WithArguments("AreEqual", "(int)1");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

    }
}
