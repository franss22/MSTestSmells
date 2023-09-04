using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;
using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
    TestSmells.AssertionRoulette.AssertionRouletteAnalyzer,
TestSmells.AssertionRoulette.AssertionRouletteCodeFixProvider >;
using System.Collections.Immutable;
using TestReading;
using System.Globalization;
using System.Collections.Generic;
using System;

namespace TestSmells.Test.AssertionRoulette
{
    [TestClass]
    public class AssertionRouletteCodefixUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("AssertionRoulette", "Corpus", "Codefix");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);

        }

        [TestMethod]
        public async Task CodeFixTest()
        {
            var testFile = @"NoMessageFirst.cs";
            var fixedFile = @"NoMessageFirstFixed.cs";

            var expected = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 39).WithArguments("AreEqual");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                FixedCode = testReader.ReadTest(fixedFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
    }
}
