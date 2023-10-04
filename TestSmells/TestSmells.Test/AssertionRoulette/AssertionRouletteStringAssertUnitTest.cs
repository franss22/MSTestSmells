using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.AssertionRoulette.AssertionRouletteAnalyzer>;

//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.AssertionRoulette.AssertionRouletteAnalyzer>;
//    TestSmells.AssertionRoulette.AssertionRouletteCodeFixProvider >;
using TestReading;

namespace TestSmells.Test.AssertionRoulette
{
    [TestClass]
    public class AssertionRouletteStringAssertUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("AssertionRoulette", "Corpus", "StringAssert");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);

        }

        

        [TestMethod]
        public async Task ContainsWithMessage()
        {
            var testFolder = "Contains";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ContainsWithoutMessage()
        {
            var testFolder = "Contains";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 40).WithArguments("Contains");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 40).WithArguments("Contains");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoesNotMatchWithMessage()
        {
            var testFolder = "DoesNotMatch";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoesNotMatchWithoutMessage()
        {
            var testFolder = "DoesNotMatch";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 45).WithArguments("DoesNotMatch");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(18, 13, 18, 45).WithArguments("DoesNotMatch");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task EndsWithWithMessage()
        {
            var testFolder = "EndsWith";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task EndsWithWithoutMessage()
        {
            var testFolder = "EndsWith";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 40).WithArguments("EndsWith");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 40).WithArguments("EndsWith");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task MatchesWithMessage()
        {
            var testFolder = "Matches";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task MatchesWithoutMessage()
        {
            var testFolder = "Matches";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 40).WithArguments("Matches");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(18, 13, 18, 40).WithArguments("Matches");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task StartsWithWithMessage()
        {
            var testFolder = "StartsWith";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task StartsWithWithoutMessage()
        {
            var testFolder = "StartsWith";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 42).WithArguments("StartsWith");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 42).WithArguments("StartsWith");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
    }
}
