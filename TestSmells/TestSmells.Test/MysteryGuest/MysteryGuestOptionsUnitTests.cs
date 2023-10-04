
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
        



        private readonly TestReader testReader = new TestReader("MysteryGuest", "Corpus", "editorconfig");

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
            var testFile = @"shouldignore.cs";

            //var diagnostic = VerifyCS.Diagnostic().WithSpan(13, 24, 13, 43).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly,
            };

            (string filename, string content) editorconfig = ("/.editorconfig", 
                @"root = true

[*.cs]

dotnet_diagnostic.MysteryGuest.IgnoredFiles = C:\Program Files\AMD\atikmdag_dce.log, C:\Program Files\AMD\atikmdag_dceb.log");

            test.TestState.AnalyzerConfigFiles.Add(editorconfig);
            await test.RunAsync();
        }

 

    }
}
