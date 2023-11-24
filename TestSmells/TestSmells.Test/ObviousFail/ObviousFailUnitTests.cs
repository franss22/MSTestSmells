using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.Compendium.AnalyzerCompendium>;

using TestReading;
using TestSmells.Compendium;

namespace TestSmells.Test.ObviousFail
{
    [TestClass]
    public class ObviousFailUnitTests

    {
        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("ObviousFail", "Corpus");

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("ObviousFail");


        [TestMethod]
        public async Task IsTrueFalse()
        {
            var testFile = @"isTrueFalse.cs";
            var expected = VerifyCS.Diagnostic("ObviousFail").WithSpan(14, 13, 14, 33).WithArguments("Assert.IsTrue(false)");
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
        public async Task IsFalseTrue()
        {
            var testFile = @"isFalseTrue.cs";
            var expected = VerifyCS.Diagnostic("ObviousFail").WithSpan(14, 13, 14, 33).WithArguments("Assert.IsFalse(true)");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

    }
}
