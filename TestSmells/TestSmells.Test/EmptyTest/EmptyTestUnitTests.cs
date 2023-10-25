using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
    TestSmells.Compendium.AnalyzerCompendium,
    TestSmells.EmptyTest.EmptyTestCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.EmptyTest
{
    [TestClass]
    public class EmptyTestUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("EmptyTest", "Corpus");

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("EmptyTest");

        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task EmptyTestReported()
        {
            var testFile = @"Emptytest.cs";
            var expected = VerifyCS.Diagnostic("EmptyTest").WithSpan(9, 21, 9, 32).WithArguments("TestMethod1");
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
        public async Task EmptyTestWithCommentReported()
        {
            var testFile = @"EmptyTestWithComments.cs";
            var expected = VerifyCS.Diagnostic("EmptyTest").WithSpan(9, 21, 9, 32).WithArguments("TestMethod1");
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
        public async Task NotEmptyTestNotReported()
        {
            var testFile = @"TestNotEmpty.cs";
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
        public async Task EmptyMethodNotTestMethodNotReported()
        {
            var testFile = @"NotTestMethod.cs";
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
        public async Task EmptyTestNotTestClassNotReported()
        {
            var testFile = @"EmptyTestNotTestClass.cs";
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
        public async Task EmptyTestFixed()
        {
            var testFile = @"Emptytest.cs";
            var fixedFile = @"EmptytestFixed.cs";

            var expected = VerifyCS.Diagnostic("EmptyTest").WithSpan(9, 21, 9, 32).WithArguments("TestMethod1");
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

        [TestMethod]
        public async Task EmptyTestWithCommentFixed()
        {
            var testFile = @"EmptyTestWithComments.cs";
            var fixedFile = @"EmptyTestWithCommentsFixed.cs";

            var expected = VerifyCS.Diagnostic("EmptyTest").WithSpan(9, 21, 9, 32).WithArguments("TestMethod1");
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
