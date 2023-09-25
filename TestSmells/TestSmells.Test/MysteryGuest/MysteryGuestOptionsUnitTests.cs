
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.MysteryGuest.MysteryGuestAnalyzer>;
//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.MagicNumber.MagicNumberAnalyzer,
//    TestSmells.MagicNumber.MagicNumberCodeFixProvider>;
using System.Collections.Immutable;
using TestReading;
using System;
using Microsoft.CodeAnalysis;

namespace TestSmells.Test.MysteryGuest
{
    [TestClass]
    public class MysteryGuestOptionsUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();
        



        private readonly TestReader testReader = new TestReader("MysteryGuest", "Corpus", "FileRead");

        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }


        [TestMethod]
        public async Task OpenRead()
        {
            var testFile = @"OpenRead.cs";

            var diagnostic = VerifyCS.Diagnostic().WithSpan(13, 24, 13, 43).WithArguments("TestMethod");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly,
                //EditorConfig = "",
            }.RunAsync();
        }

 

    }
}
