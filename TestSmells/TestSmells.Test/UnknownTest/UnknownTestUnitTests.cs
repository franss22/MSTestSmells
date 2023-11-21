using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.Compendium.AnalyzerCompendium>;
using TestReading;
using System.Threading;
using System.IO;

namespace TestSmells.Test.UnknownTest
{
    [TestClass]
    public class UnknownTestUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();
        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("UnknownTest");

        private readonly TestReader testReader = new TestReader("UnknownTest", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task SimpleUnknownTest()
        {
            var testFile = @"SimpleUnknownTest.cs";
            var expected = VerifyCS.Diagnostic("UnknownTest").WithSpan(12, 21, 12, 32).WithArguments("TestMethod1");
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
        public async Task TestWithAssertion()
        {
            var testFile = @"TestWithAssertion.cs";
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }


        [TestMethod]
        public async Task TestWithHelperAssertion()
        {
            var testFile = @"TestWithHelperAssertion.cs";
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }


        [TestMethod]
        public async Task TestWithHelperAssertionInOptions()
        {
            var testFile = @"TestWithHelperAssertion.cs";
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            };
            var helperAssertions = "\ndotnet_diagnostic.MysteryGuest.CustomAssertions = MyTestFunction, YourTestFunction";

            (string filename, string content) editorconfig =
                (
                ExcludeOtherCompendiumDiagnostics.filename,
                ExcludeOtherCompendiumDiagnostics.content + helperAssertions
                );

            test.TestState.AnalyzerConfigFiles.Add(editorconfig);
            await test.RunAsync();
        }

    }
}
