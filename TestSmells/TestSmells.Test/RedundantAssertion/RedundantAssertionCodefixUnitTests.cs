using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
    TestSmells.Compendium.AnalyzerCompendium,
    TestSmells.RedundantAssertion.RedundantAssertionCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.RedundantAssertion
{
    [TestClass]
    public class RedundantAssertionCodefixUnitTests

    {
        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("RedundantAssertion", "Corpus");

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("RedundantAssertion");


        [TestMethod]
        public async Task SameIdentifier()
        {
            var testFile = @"SameIdentifier.cs";
            var fixedFile = @"SameIdentifierFixed.cs";
            var expected = VerifyCS.Diagnostic("RedundantAssertion").WithSpan(13, 13, 13, 34).WithArguments("TestMethod1", "AreEqual");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                FixedCode = testReader.ReadTest(fixedFile),
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task DeletesComments()
        {
            var testFile = @"SameButWithComments.cs";
            var fixedFile = @"SameIdentifierFixed.cs";
            var expected = VerifyCS.Diagnostic("RedundantAssertion").WithSpan(13, 13, 13, 44).WithArguments("TestMethod1", "AreEqual");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                FixedCode = testReader.ReadTest(fixedFile),
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

    }
}
