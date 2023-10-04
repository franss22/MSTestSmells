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
    public class AssertionRouletteAssertUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("AssertionRoulette", "Corpus", "Assert");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);

        }

        [TestMethod]
        public async Task AreEqualSingleAssertOneMessageWithOneParam()
        {
            var testFolder = "AreEqual";
            var testFile = @"MessageParamExtra.cs";

            var expected = VerifyCS.Diagnostic().WithSpan(16, 13, 16, 34).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualDoubleAssertDoubleMessageParams()
        {
            var testFolder = "AreEqual";
            var testFile = @"DoubleAssertDoubleMessageParams.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualDoubleAssertDoubleNoMessageParamsFirst()
        {
            var testFolder = "AreEqual";
            var testFile = @"DoubleAssertDoubleNoMessageParamsFirst.cs";

            var expected = VerifyCS.Diagnostic().WithSpan(13, 13, 13, 39).WithArguments("AreEqual");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualDoubleAssertDoubleNoMessageParamsSecond()
        {
            var testFolder = "AreEqual";
            var testFile = @"DoubleAssertDoubleNoMessageParamsSecond.cs";

            var expected = VerifyCS.Diagnostic().WithSpan(17, 13, 17, 39).WithArguments("AreEqual");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualSingleAssertDoubleMessageParams()
        {
            var testFolder = "AreEqual";
            var testFile = @"SingleAssertDoubleMessageParams.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task AreEqualSingleAssertDoubleMessage()
        {
            var testFolder = "AreEqual";
            var testFile = @"SingleAssertDoubleMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualSingleAssertDouble()
        {
            var testFolder = "AreEqual";
            var testFile = @"SingleAssertDouble.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualDoubleAssertDoubleNoMessageFirst()
        {
            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 39).WithArguments("AreEqual");

            var testFolder = "AreEqual";
            var testFile = @"DoubleAssertDoubleNoMessageFirst.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualDoubleAssertDoubleNoMessageSecond()
        {
            var testFolder = "AreEqual";
            var testFile = @"DoubleAssertDoubleNoMessageSecond.cs";

            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 39).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualDoubleAssertDoubleNoMessageBoth()
        {
            var testFolder = "AreEqual";
            var testFile = @"DoubleAssertDoubleNoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 39).WithArguments("AreEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 39).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualDoubleAssertDoubleMessage()
        {
            var testFolder = "AreEqual";
            var testFile = @"DoubleAssertDoubleMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task AreNotEqualWithMessage()
        {
            var testFolder = "AreNotEqual";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEqualWithoutMessage()
        {
            var testFolder = "AreNotEqual";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 42).WithArguments("AreNotEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 42).WithArguments("AreNotEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task AreSameWithMessage()
        {
            var testFolder = "AreSame";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreSameWithoutMessage()
        {
            var testFolder = "AreSame";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 33).WithArguments("AreSame");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 33).WithArguments("AreSame");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task AreNotSameWithMessage()
        {
            var testFolder = "AreNotSame";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreNotSameWithoutMessage()
        {
            var testFolder = "AreNotSame";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 36).WithArguments("AreNotSame");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 36).WithArguments("AreNotSame");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task FailWithMessage()
        {
            var testFolder = "Fail";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task FailWithoutMessage()
        {
            var testFolder = "Fail";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(12, 13, 12, 26).WithArguments("Fail");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 26).WithArguments("Fail");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task InconclusiveWithMessage()
        {
            var testFolder = "Inconclusive";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task InconclusiveWithoutMessage()
        {
            var testFolder = "Inconclusive";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(12, 13, 12, 34).WithArguments("Inconclusive");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 34).WithArguments("Inconclusive");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task IsFalseWithMessage()
        {
            var testFolder = "IsFalse";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsFalseWithoutMessage()
        {
            var testFolder = "IsFalse";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(12, 13, 12, 30).WithArguments("IsFalse");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 30).WithArguments("IsFalse");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task IsInstanceOfTypeWithMessage()
        {
            var testFolder = "IsInstanceOfType";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsInstanceOfTypeWithoutMessage()
        {
            var testFolder = "IsInstanceOfType";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(12, 13, 12, 52).WithArguments("IsInstanceOfType");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 52).WithArguments("IsInstanceOfType");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task IsNotInstanceOfTypeWithMessage()
        {
            var testFolder = "IsNotInstanceOfType";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsNotInstanceOfTypeWithoutMessage()
        {
            var testFolder = "IsNotInstanceOfType";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(12, 13, 12, 55).WithArguments("IsNotInstanceOfType");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 55).WithArguments("IsNotInstanceOfType");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task IsNotNullWithMessage()
        {
            var testFolder = "IsNotNull";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsNotNullWithoutMessage()
        {
            var testFolder = "IsNotNull";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(12, 13, 12, 32).WithArguments("IsNotNull");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 32).WithArguments("IsNotNull");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task IsNullWithMessage()
        {
            var testFolder = "IsNull";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsNullWithoutMessage()
        {
            var testFolder = "IsNull";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(12, 13, 12, 29).WithArguments("IsNull");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 29).WithArguments("IsNull");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task ThrowsExceptionWithMessage()
        {
            var testFolder = "ThrowsException";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ThrowsExceptionWithoutMessage()
        {
            var testFolder = "ThrowsException";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 74).WithArguments("ThrowsException");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(16, 13, 16, 74).WithArguments("ThrowsException");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ThrowsExceptionAsyncWithMessage()
        {
            var testFolder = "ThrowsExceptionAsync";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ThrowsExceptionAsyncWithoutMessage()
        {
            var testFolder = "ThrowsExceptionAsync";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(26, 13, 26, 76).WithArguments("ThrowsExceptionAsync");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(29, 13, 29, 76).WithArguments("ThrowsExceptionAsync");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AssertInsideIf()
        {
            var testFolder = "EdgeCases";
            var testFile = @"AssertsInNestedBlocks.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(16, 17, 16, 43).WithArguments("AreEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(23, 17, 23, 43).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
    }
}
