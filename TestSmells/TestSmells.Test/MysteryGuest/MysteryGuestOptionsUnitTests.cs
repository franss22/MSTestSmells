
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.Compendium.AnalyzerCompendium>;

//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.MagicNumber.MagicNumberAnalyzer,
//    TestSmells.MagicNumber.MagicNumberCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.MysteryGuest
{
    [TestClass]
    public class MysteryGuestOptionsUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("MysteryGuest");



        private readonly TestReader testReader = new TestReader("MysteryGuest", "Corpus", "editorconfig");

        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }


        [TestMethod]
        public async Task ShouldIgnoreVar()
        {
            var testFile = @"ShouldIgnoreVar.cs";

            //var diagnostic = VerifyCS.Diagnostic().WithSpan(13, 24, 13, 43).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly,
            };

            var ignoredFiles = "\ndotnet_diagnostic.MysteryGuest.IgnoredFiles = C:\\Program Files\\AMD\\atikmdag_dce.log, C:\\Program Files\\AMD\\atikmdag_dceb.log";

            (string filename, string content) editorconfig = 
                (
                ExcludeOtherCompendiumDiagnostics.filename, 
                ExcludeOtherCompendiumDiagnostics.content+ignoredFiles
                );

            test.TestState.AnalyzerConfigFiles.Add(editorconfig);
            await test.RunAsync();
        }


        [TestMethod]
        public async Task ShouldIgnoreArgument()
        {
            var testFile = @"ShouldIgnoreArgument.cs";

            //var diagnostic = VerifyCS.Diagnostic().WithSpan(13, 24, 13, 43).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly,
            };

            var ignoredFiles = "\ndotnet_diagnostic.MysteryGuest.IgnoredFiles = C:\\Program Files\\AMD\\atikmdag_dce.log, C:\\Program Files\\AMD\\atikmdag_dceb.log";

            (string filename, string content) editorconfig =
                (
                ExcludeOtherCompendiumDiagnostics.filename,
                ExcludeOtherCompendiumDiagnostics.content + ignoredFiles
                );

            test.TestState.AnalyzerConfigFiles.Add(editorconfig);
            await test.RunAsync();
        }


    }
}
