﻿using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.Compendium.AnalyzerCompendium>;
using TestReading;

namespace TestSmells.Test.DuplicateAssert
{
    [TestClass]
    public class DuplicateAssertUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("DuplicateAssert");


        private readonly TestReader testReader = new TestReader("DuplicateAssert", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }



        [TestMethod]
        public async Task SimpleDuplicateAssert()
        {
            var testFile = @"SimpleDuplicateAssert.cs";
            var expected = VerifyCS.Diagnostic("DuplicateAssert")
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(11, 21, 11, 32)//method
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(18, 13, 18, 34)//2nd assert
                .WithArguments("TestMethod1", "16, 18");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task DuplicateAndNot()
        {
            //@ duplicate assertoins followed by a unique assertion

            var testFile = @"DuplicateAndNot.cs";
            var expected = VerifyCS.Diagnostic("DuplicateAssert")
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(11, 21, 11, 32)//method
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(18, 13, 18, 34)//2nd assert
                .WithArguments("TestMethod1", "16, 18");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task CommentDifference()
        {
            var testFile = @"CommentDifference.cs";
            var expected = VerifyCS.Diagnostic("DuplicateAssert")
                .WithSpan(16, 13, 16, 34)
                .WithSpan(11, 21, 11, 32)
                .WithSpan(16, 13, 16, 34)
                .WithSpan(18, 13, 18, 70)
                .WithArguments("TestMethod1", "16, 18");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task DifferentArguments()
        {
            var testFile = @"DifferentArguments.cs";
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task DifferentMethods()
        {
            var testFile = @"DifferentMethods.cs";
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task DoubleDiagnostic()
        {
            var testFile = @"DoubleDiagnostic.cs";
            var expected = VerifyCS.Diagnostic("DuplicateAssert")
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(11, 21, 11, 32)//method
                .WithSpan(16, 13, 16, 34)//1st assert
                .WithSpan(18, 13, 18, 34)//2nd assert
                .WithArguments("TestMethod1", "16, 18");
            var expected2 = VerifyCS.Diagnostic("DuplicateAssert")
                .WithSpan(20, 13, 20, 46)
                .WithSpan(11, 21, 11, 32)
                .WithSpan(20, 13, 20, 46)
                .WithSpan(22, 13, 22, 46)
                .WithArguments("TestMethod1", "20, 22");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected, expected2 },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task WhitespaceDifference()
        {
            var testFile = @"WhitespaceDifference.cs";
            var expected = VerifyCS.Diagnostic("DuplicateAssert")
                .WithSpan(16, 13, 16, 44)
                .WithSpan(11, 21, 11, 32)
                .WithSpan(16, 13, 16, 44)
                .WithSpan(18, 16, 18, 43)
                .WithArguments("TestMethod1", "16, 18");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }



    }
}
