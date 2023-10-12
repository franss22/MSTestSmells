using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
    TestSmells.IgnoredTest.IgnoredTestAnalyzer,
    TestSmells.IgnoredTest.IgnoredTestCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.IgnoredTest
{
    [TestClass]
    public class IgnoredTestCodefixUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("IgnoredTest", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task SimpleIgnoredTest()
        {
            var testFile = @"SimpleIgnoredTest.cs";
            var fixedFile = @"SimpleIgnoredTestFixed.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(11, 10, 11, 16).WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IgnoredTestInListFirst()
        {
            var testFile = @"IgnoredTestInListFirst.cs";
            var fixedFile = @"IgnoredTestInListFirstFixed.cs";

            var expected = VerifyCS.Diagnostic().WithSpan(10, 10, 10, 16).WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IgnoredTestInListLast()
        {
            var testFile = @"IgnoredTestInListLast.cs";
            var fixedFile = @"IgnoredTestInListLastFixed.cs";

            var expected = VerifyCS.Diagnostic().WithSpan(10, 22, 10, 28).WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IgnoredTestInListMiddle()
        {
            var testFile = @"IgnoredTestInListMiddle.cs";
            var fixedFile = @"IgnoredTestInListMiddleFixed.cs";

            var expected = VerifyCS.Diagnostic().WithSpan(10, 22, 10, 28).WithArguments("TestMethod1");
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
