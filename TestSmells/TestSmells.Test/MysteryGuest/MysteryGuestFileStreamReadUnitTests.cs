
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.MysteryGuest.MysteryGuestAnalyzer>;
//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.MagicNumber.MagicNumberAnalyzer,
//    TestSmells.MagicNumber.MagicNumberCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.MysteryGuest
{
    [TestClass]
    public class MysteryGuestFileStreamReadUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();
        



        private readonly TestReader testReader = new TestReader("MysteryGuest", "Corpus", "FileStreamRead");

        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BeginRead()
        {
            var testFile = @"BeginRead.cs";

            var diagnostic = VerifyCS.Diagnostic().WithSpan(17, 24, 17, 64).WithArguments("TestMethod");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task EndRead()
        {
            var testFile = @"EndRead.cs";

            var diagnostic = VerifyCS.Diagnostic().WithSpan(16, 24, 16, 47).WithArguments("TestMethod");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task Read()
        {
            var testFile = @"Read.cs";

            var diagnostic = VerifyCS.Diagnostic().WithSpan(16, 24, 16, 44).WithArguments("TestMethod");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ReadAsync()
        {
            var testFile = @"ReadAsync.cs";

            var diagnostic = VerifyCS.Diagnostic().WithSpan(16, 30, 16, 55).WithArguments("TestMethod");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ReadByte()
        {
            var testFile = @"ReadByte.cs";

            var diagnostic = VerifyCS.Diagnostic().WithSpan(15, 24, 15, 39).WithArguments("TestMethod");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

 

    }
}
