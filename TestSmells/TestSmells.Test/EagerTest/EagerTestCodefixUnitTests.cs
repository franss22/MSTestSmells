using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
    TestSmells.Compendium.AnalyzerCompendium,
    TestSmells.EagerTest.EagerTestCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.EagerTest
{
    [TestClass]
    public class EagerTestCodefixUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("EagerTest", "Corpus");

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("EagerTest");

        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task SimpleEagerTest()
        {
            var testFile = @"SimpleEagerTest.cs";
            var fixedFile = @"SimpleEagerTestFixed.cs";

            var expected = VerifyCS.Diagnostic("EagerTest")
                .WithSpan(15, 13, 15, 41)
                .WithSpan(10, 21, 10, 32)
                .WithSpan(15, 13, 15, 41)
                .WithSpan(16, 13, 16, 39)
                .WithArguments("TestMethod1");
            ;
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
        public async Task LocalVarEagerTest()
        {
            var testFile = @"LocalVarEagerTest.cs";
            var fixedFile = @"LocalVarEagerTestFixed.cs";

            var expected = VerifyCS.Diagnostic("EagerTest")
                .WithSpan(18, 13, 18, 31) //First Assert
                .WithSpan(10, 21, 10, 32) //Method Declaration
                .WithSpan(18, 13, 18, 31) //First Assert
                .WithSpan(19, 13, 19, 31) //Second Assert
                .WithArguments("TestMethod1");
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
        public async Task MultipleArguments()
        {
            var testFile = @"MultipleArguments.cs";
            var fixedFile = @"MultipleArgumentsFixed.cs";

            var expected = VerifyCS.Diagnostic("EagerTest")
                .WithSpan(16, 13, 16, 67)
                .WithSpan(10, 21, 10, 32)
                .WithSpan(16, 13, 16, 67)
                .WithSpan(17, 13, 17, 64)
                .WithArguments("TestMethod1");
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
        public async Task LocalVarEagerTestTrivia()
        {
            var testFile = @"LocalVarEagerTestTrivia.cs";
            var fixedFile = @"LocalVarEagerTestTriviaFixed.cs";

            var expected = VerifyCS.Diagnostic("EagerTest")
                .WithSpan(18, 13, 18, 36)
                .WithSpan(10, 21, 10, 32)
                .WithSpan(18, 13, 18, 36)
                .WithSpan(19, 13, 19, 31)
                .WithArguments("TestMethod1");
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
