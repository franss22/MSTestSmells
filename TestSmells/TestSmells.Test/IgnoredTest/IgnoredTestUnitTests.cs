using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.DuplicateAssert.DuplicateAssertAnalyzer>;
using TestReading;

namespace TestSmells.Test.DuplicateAssert
{
    [TestClass]
    public class DuplicateAssertUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("DuplicateAssert", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task SimpleDuplicateAssert()
        {
            var testFile = @"SimpleDuplicateAssert.cs";
            var expected = VerifyCS.Diagnostic()
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(11, 21, 11, 32)//method
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(18, 13, 18, 34)//2nd assert
                .WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task CommentDifference()
        {
            var testFile = @"CommentDifference.cs";
            var expected = VerifyCS.Diagnostic()
                .WithSpan(16, 13, 16, 34)
                .WithSpan(11, 21, 11, 32)
                .WithSpan(16, 13, 16, 34)
                .WithSpan(18, 13, 18, 70)
                .WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DifferentArguments()
        {
            var testFile = @"DifferentArguments.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DifferentMethods()
        {
            var testFile = @"DifferentMethods.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleDiagnostic()
        {
            var testFile = @"DoubleDiagnostic.cs";
            var expected = VerifyCS.Diagnostic()
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(11, 21, 11, 32)//method
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(18, 13, 18, 34)//2nd assert
                .WithArguments("TestMethod1");
            var expected2 = VerifyCS.Diagnostic()
                .WithSpan(20, 13, 20, 46)
                .WithSpan(11, 21, 11, 32)
                .WithSpan(20, 13, 20, 46)
                .WithSpan(22, 13, 22, 46)
                .WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected, expected2 },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task WhitespaceDifference()
        {
            var testFile = @"WhitespaceDifference.cs";
            var expected = VerifyCS.Diagnostic()
                .WithSpan(16, 13, 16, 44)
                .WithSpan(11, 21, 11, 32)
                .WithSpan(16, 13, 16, 44)
                .WithSpan(18, 16, 18, 43)
                .WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }



    }
}
