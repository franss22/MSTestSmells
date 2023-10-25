using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.Compendium.AnalyzerCompendium>;
using TestReading;

namespace TestSmells.Test.SleepyTest
{
    [TestClass]
    public class SleepyTestUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("SleepyTest", "Corpus");

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("SleepyTest");

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
            var expected = VerifyCS.Diagnostic("SleepyTest").WithSpan(16, 13, 16, 31).WithArguments("TestMethod1");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task TimespanSleepyTest()
        {
            var testFile = @"TimespanSleepyTest.cs";
            var expected = VerifyCS.Diagnostic("SleepyTest").WithSpan(20, 13, 20, 35).WithArguments("TestMethod1");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task TestWithoutSleep()
        {
            var testFile = @"TestWithoutSleep.cs"; var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }


    }
}
