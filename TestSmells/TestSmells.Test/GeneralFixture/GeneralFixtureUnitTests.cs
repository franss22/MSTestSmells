
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.GeneralFixture.GeneralFixtureAnalyzer>;
//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.MagicNumber.MagicNumberAnalyzer,
//    TestSmells.MagicNumber.MagicNumberCodeFixProvider>;
using System.Collections.Immutable;
using TestReading;
using System;
using Microsoft.CodeAnalysis;

namespace TestSmells.Test.GeneralFixture
{
    [TestClass]
    public class GeneralFixtureUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("GeneralFixture", "Corpus");




        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestClass()
        {
            var testFile = @"TestClass.cs";
            var diagnosticField1 = VerifyCS.Diagnostic().WithSpan(16, 13, 16, 27).WithArguments("field_num1", "TestMethod");

            var diagnosticField2 = VerifyCS.Diagnostic().WithSpan(19, 13, 19, 34).WithArguments("field_num2", "TestMethod");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnosticField1, diagnosticField2 },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task NoFieldsUsed()
        {
            var testFile = @"NoFieldsUsed.cs";

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = {
            VerifyCS.Diagnostic().WithSpan(16, 13, 16, 27).WithArguments("field_num1", "TestMethod"),
            VerifyCS.Diagnostic().WithSpan(16, 13, 16, 27).WithArguments("field_num1", "TestMethod1"),
            VerifyCS.Diagnostic().WithSpan(17, 13, 17, 27).WithArguments("field_num2", "TestMethod"),
            VerifyCS.Diagnostic().WithSpan(17, 13, 17, 27).WithArguments("field_num2", "TestMethod1"),
            VerifyCS.Diagnostic().WithSpan(18, 13, 18, 27).WithArguments("field_num3", "TestMethod"),
            VerifyCS.Diagnostic().WithSpan(18, 13, 18, 27).WithArguments("field_num3", "TestMethod1"),
            },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AllFieldsUsed()
        {
            var testFile = @"AllFieldsUsed.cs";

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


    }
}
