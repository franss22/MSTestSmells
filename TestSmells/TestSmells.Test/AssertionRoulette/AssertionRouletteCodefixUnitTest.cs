using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
    TestSmells.Compendium.AnalyzerCompendium,
TestSmells.AssertionRoulette.AssertionRouletteCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.AssertionRoulette
{
    [TestClass]
    public class AssertionRouletteCodefixUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("AssertionRoulette", "Corpus", "Codefix");
        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("AssertionRoulette");

        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);

        }

        [TestMethod]
        public async Task CodeFixTest()
        {
            var testFile = @"NoMessageFirst.cs";
            var fixedFile = @"NoMessageFirstFixed.cs";

            var expected = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 39).WithArguments("AreEqual");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
    }
}
