﻿using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.Compendium.AnalyzerCompendium>;
using TestReading;

namespace TestSmells.Test.ConditionalTest
{
    [TestClass]
    public class ConditionalTestUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("ConditionalTest", "Corpus");

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("ConditionalTest");

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
            var expected = VerifyCS.Diagnostic("ConditionalTest")
                .WithSpan(14, 13, 17, 14)
                .WithArguments("TestMethod1", "conditional");

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
        public async Task Do()
        {
            var testFile = @"Do.cs";
            var expected = VerifyCS.Diagnostic("ConditionalTest").WithSpan(14, 13, 17, 35).WithArguments("TestMethod1", "loop");

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
        public async Task For()
        {
            var testFile = @"For.cs";
            var expected = VerifyCS.Diagnostic("ConditionalTest").WithSpan(14, 13, 17, 14).WithArguments("TestMethod1", "loop")
;

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
        public async Task Foreach()
        {
            var testFile = @"Foreach.cs";

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
        public async Task IfElse()
        {
            var testFile = @"IfElse.cs";
            var expected = VerifyCS.Diagnostic("ConditionalTest").WithSpan(14, 13, 21, 14).WithArguments("TestMethod1", "conditional");

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
        public async Task Switch()
        {
            var testFile = @"Switch.cs";
            var expected = VerifyCS.Diagnostic("ConditionalTest").WithSpan(15, 13, 26, 14).WithArguments("TestMethod1", "switch");

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
        public async Task TernaryIf()
        {
            var testFile = @"TernaryIf.cs";
            var expected = VerifyCS.Diagnostic("ConditionalTest").WithSpan(14, 20, 14, 39).WithArguments("TestMethod1", "conditional");

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
        public async Task While()
        {
            var testFile = @"While.cs";
            var expected = VerifyCS.Diagnostic("ConditionalTest").WithSpan(14, 13, 17, 14).WithArguments("TestMethod1", "loop");

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
