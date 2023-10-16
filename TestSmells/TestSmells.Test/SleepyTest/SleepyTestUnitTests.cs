using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.SleepyTest.SleepyTestAnalyzer>;
using TestReading;

namespace TestSmells.Test.SleepyTest
{
    [TestClass]
    public class SleepyTestUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("SleepyTest", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task SimpleSleepyTest()
        {
            var testFile = @"SimpleSleepyTest.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(16, 13, 16, 31).WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task TimespanSleepyTest()
        {
            var testFile = @"TimespanSleepyTest.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(20, 13, 20, 35).WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }



    }
}
