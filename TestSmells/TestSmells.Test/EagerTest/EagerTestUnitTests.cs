using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.EagerTest.EagerTestAnalyzer>;

using System.Collections.Immutable;
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
        public async Task EagerTestReported()
        {
            var testFile = @"EagerTest.cs";
            var expected = VerifyCS.Diagnostic("EagerTest").WithSpan(9, 21, 9, 32).WithArguments("TestMethod1");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();

        }

        [TestMethod]
        public async Task NoEagerTestReported()
        {
            var testFile = @"EagerTest.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = {},
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();

        }


    }
}
