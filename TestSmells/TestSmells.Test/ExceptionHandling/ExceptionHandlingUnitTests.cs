using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.ExceptionHandling.ExceptionHandlingAnalyzer>;
using TestReading;

namespace TestSmells.Test.ExceptionHandling
{
    [TestClass]
    public class ExceptionHandlingUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("ExceptionHandling", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task TryCatch()
        {
            var testFile = @"TryCatch.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(16, 13, 23, 14).WithArguments("TestMethod1", "handles exceptions");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task Throw()
        {
            var testFile = @"Throw.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(19, 13, 19, 53).WithArguments("TestMethod1", "throws an exception");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task TryCatchFinally()
        {
            var testFile = @"TryCatchFinally.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(16, 13, 23, 14).WithArguments("TestMethod1", "handles exceptions");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task Cleantest()
        {
            var testFile = @"Cleantest.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = {  },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }



    }
}
