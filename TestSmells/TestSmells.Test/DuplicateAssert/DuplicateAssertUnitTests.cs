﻿using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.DuplicateAssert.DuplicateAssertAnalyzer>;
using TestReading;

namespace TestSmells.Test.DuplicateAssert
{
    [TestClass]
    public class DuplicateAssertUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("DuplicateAssert", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task SimpleDeadField()
        {
            var testFile = @"SimpleDeadField.cs";
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



    }
}
