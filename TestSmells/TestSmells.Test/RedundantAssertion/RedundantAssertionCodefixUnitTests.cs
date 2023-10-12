using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
    TestSmells.RedundantAssertion.RedundantAssertionAnalyzer,
    TestSmells.RedundantAssertion.RedundantAssertionCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.RedundantAssertion
{
    [TestClass]
    public class RedundantAssertionCodefixUnitTests

    {
        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("RedundantAssertion", "Corpus");

        [TestMethod]
        public async Task SameIdentifier()
        {
            var testFile = @"SameIdentifier.cs";
            var fixedFile = @"SameIdentifierFixed.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(13, 13, 13, 34).WithArguments("TestMethod1", "AreEqual");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DeletesComments()
        {
            var testFile = @"SameButWithComments.cs";
            var fixedFile = @"SameIdentifierFixed.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(13, 13, 13, 44).WithArguments("TestMethod1", "AreEqual");
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
