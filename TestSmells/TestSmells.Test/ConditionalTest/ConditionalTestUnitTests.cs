using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.ConditionalTest.ConditionalTestAnalyzer>;
using TestReading;

namespace TestSmells.Test.ConditionalTest
{
    [TestClass]
    public class ConditionalTestUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("ConditionalTest", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task SimpleIf()
        {
            var testFile = @"SimpleIf.cs";
            var expected = VerifyCS.Diagnostic()
                .WithSpan(14, 13, 17, 14)
                .WithArguments("TestMethod1", "conditional");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task Do()
        {
            var testFile = @"Do.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(14, 13, 17, 35).WithArguments("TestMethod1", "loop");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task For()
        {
            var testFile = @"For.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(14, 13, 17, 14).WithArguments("TestMethod1", "loop")
;

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task Foreach()
        {
            var testFile = @"Foreach.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(14, 13, 17, 14).WithArguments("TestMethod1", "loop");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IfElse()
        {
            var testFile = @"IfElse.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(14, 13, 21, 14).WithArguments("TestMethod1", "conditional");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task Switch()
        {
            var testFile = @"Switch.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(15, 13, 26, 14).WithArguments("TestMethod1", "switch");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task TernaryIf()
        {
            var testFile = @"TernaryIf.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(14, 20, 14, 39).WithArguments("TestMethod1", "conditional");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task While()
        {
            var testFile = @"While.cs";
            var expected = VerifyCS.Diagnostic().WithSpan(14, 13, 17, 14).WithArguments("TestMethod1", "loop");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }






    }
}
