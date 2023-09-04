using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.AssertionRoulette.AssertionRouletteAnalyzer>;

//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.AssertionRoulette.AssertionRouletteAnalyzer>;
//    TestSmells.AssertionRoulette.AssertionRouletteCodeFixProvider >;
using System.Collections.Immutable;
using TestReading;
using System.Globalization;
using System.Collections.Generic;
using System;

namespace TestSmells.Test.AssertionRoulette
{
    [TestClass]
    public class AssertionRouletteCollectionAssertUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("AssertionRoulette", "Corpus", "CollectionAssert");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);

        }

        [TestMethod]
        public async Task AllItemsAreInstancesOfTypeWithMessage()
        {
            var testFolder = "AllItemsAreInstancesOfType";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreInstancesOfTypeWithoutMessage()
        {
            var testFolder = "AllItemsAreInstancesOfType";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 72).WithArguments("AllItemsAreInstancesOfType");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 72).WithArguments("AllItemsAreInstancesOfType");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreNotNullWithMessage()
        {
            var testFolder = "AllItemsAreNotNull";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreNotNullWithoutMessage()
        {
            var testFolder = "AllItemsAreNotNull";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 51).WithArguments("AllItemsAreNotNull");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 51).WithArguments("AllItemsAreNotNull");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreUniqueWithMessage()
        {
            var testFolder = "AllItemsAreUnique";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreUniqueWithoutMessage()
        {
            var testFolder = "AllItemsAreUnique";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 50).WithArguments("AllItemsAreUnique");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 50).WithArguments("AllItemsAreUnique");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualWithMessage()
        {
            var testFolder = "AreEqual";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualWithoutMessage()
        {
            var testFolder = "AreEqual";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 44).WithArguments("AreEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 44).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEquivalentWithMessage()
        {
            var testFolder = "AreEquivalent";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreEquivalentWithoutMessage()
        {
            var testFolder = "AreEquivalent";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 49).WithArguments("AreEquivalent");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 49).WithArguments("AreEquivalent");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEqualWithMessage()
        {
            var testFolder = "AreNotEqual";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEqualWithoutMessage()
        {
            var testFolder = "AreNotEqual";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 47).WithArguments("AreNotEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 47).WithArguments("AreNotEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEquivalentWithMessage()
        {
            var testFolder = "AreNotEquivalent";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEquivalentWithoutMessage()
        {
            var testFolder = "AreNotEquivalent";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 52).WithArguments("AreNotEquivalent");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 52).WithArguments("AreNotEquivalent");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ContainsWithMessage()
        {
            var testFolder = "Contains";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ContainsWithoutMessage()
        {
            var testFolder = "Contains";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 44).WithArguments("Contains");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 44).WithArguments("Contains");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoesNotContainWithMessage()
        {
            var testFolder = "DoesNotContain";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoesNotContainWithoutMessage()
        {
            var testFolder = "DoesNotContain";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 50).WithArguments("DoesNotContain");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 50).WithArguments("DoesNotContain");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsNotSubsetOfWithMessage()
        {
            var testFolder = "IsNotSubsetOf";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsNotSubsetOfWithoutMessage()
        {
            var testFolder = "IsNotSubsetOf";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 49).WithArguments("IsNotSubsetOf");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 49).WithArguments("IsNotSubsetOf");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsSubsetOfWithMessage()
        {
            var testFolder = "IsSubsetOf";
            var testFile = @"MessageBoth.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task IsSubsetOfWithoutMessage()
        {
            var testFolder = "IsSubsetOf";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 46).WithArguments("IsSubsetOf");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 46).WithArguments("IsSubsetOf");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }



    }
}
