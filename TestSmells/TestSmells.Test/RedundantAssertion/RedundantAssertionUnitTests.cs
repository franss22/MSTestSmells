using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.RedundantAssertion.RedundantAssertionAnalyzer>;
using TestReading;

namespace TestSmells.Test.RedundantAssertion
{
    [TestClass]
    public class RedundantAssertionUnitTests

    {
        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("RedundantAssertion", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task SameIdentifier()
        {
            var testFile = @"SameIdentifier.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(13, 13, 13, 34).WithArguments("TestMethod1", "AreEqual");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task Different()
        {
            var testFile = @"Different.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SameButWithComments()
        {
            var testFile = @"SameButWithComments.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(13, 13, 13, 44).WithArguments("TestMethod1", "AreEqual");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SameMethod()
        {
            var testFile = @"SameMethod.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(13, 13, 13, 54).WithArguments("TestMethod1", "AreEqual");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly,
            }.RunAsync();
        }

        [TestMethod]
        public async Task Contains()
        {
            var testFile = @"Contains.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(13, 13, 13, 40).WithArguments("TestMethod1", "Contains");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly,
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEqual()
        {
            var testFile = @"AreNotEqual.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(13, 13, 13, 34).WithArguments("TestMethod1", "AreEqual");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly,
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsSubsetOf()
        {
            var testFile = @"IsSubsetOf.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(13, 13, 13, 34).WithArguments("TestMethod1", "AreEqual");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly,
            }.RunAsync();
        }


    }
}
