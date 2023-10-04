using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.EagerTest.EagerTestAnalyzer>;
using TestReading;

namespace TestSmells.Test.EagerTest
{
    [TestClass]
    public class EagerTestUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("EagerTest", "Corpus");
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
            var expected = VerifyCS.Diagnostic()
                .WithSpan(16, 13, 16, 41) //First Assert
                .WithSpan(10, 21, 10, 32) //Method Declaration
                .WithSpan(16, 13, 16, 41) //First Assert
                .WithSpan(17, 13, 17, 39) //Second Assert
                .WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task LocalVarEagerTest()
        {
            var testFile = @"LocalVarEagerTest.cs";
            var expected = VerifyCS.Diagnostic()
                .WithSpan(18, 13, 18, 31) //First Assert
                .WithSpan(10, 21, 10, 32) //Method Declaration
                .WithSpan(18, 13, 18, 31) //First Assert
                .WithSpan(19, 13, 19, 31) //Second Assert
                .WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task NoSmellTwoAssertsSameMethod()
        {
            var testFile = @"TwoAssertsSameMethod.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = {},
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task NoSmellTwoAssertsSameMethodLocalVar()
        {
            var testFile = @"TwoAssertsSameMethodLocalVar.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task NoSmellTwoMethodsSameAssert()
        {
            var testFile = @"TwoMethodsSameAssert.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


    }
}
