using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.Compendium.AnalyzerCompendium>;

//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.AssertionRoulette.AssertionRouletteAnalyzer>;
//    TestSmells.AssertionRoulette.AssertionRouletteCodeFixProvider >;
using TestReading;

namespace TestSmells.Test.AssertionRoulette
{
    [TestClass]
    public class AssertionRouletteCollectionAssertUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("AssertionRoulette");

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
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreInstancesOfTypeWithoutMessage()
        {
            var testFolder = "AllItemsAreInstancesOfType";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 72).WithArguments("AllItemsAreInstancesOfType");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 72).WithArguments("AllItemsAreInstancesOfType");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreNotNullWithMessage()
        {
            var testFolder = "AllItemsAreNotNull";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreNotNullWithoutMessage()
        {
            var testFolder = "AllItemsAreNotNull";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 51).WithArguments("AllItemsAreNotNull");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 51).WithArguments("AllItemsAreNotNull");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreUniqueWithMessage()
        {
            var testFolder = "AllItemsAreUnique";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AllItemsAreUniqueWithoutMessage()
        {
            var testFolder = "AllItemsAreUnique";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 50).WithArguments("AllItemsAreUnique");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 50).WithArguments("AllItemsAreUnique");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualWithMessage()
        {
            var testFolder = "AreEqual";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AreEqualWithoutMessage()
        {
            var testFolder = "AreEqual";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 44).WithArguments("AreEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 44).WithArguments("AreEqual");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AreEquivalentWithMessage()
        {
            var testFolder = "AreEquivalent";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AreEquivalentWithoutMessage()
        {
            var testFolder = "AreEquivalent";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 49).WithArguments("AreEquivalent");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 49).WithArguments("AreEquivalent");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEqualWithMessage()
        {
            var testFolder = "AreNotEqual";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEqualWithoutMessage()
        {
            var testFolder = "AreNotEqual";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 47).WithArguments("AreNotEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 47).WithArguments("AreNotEqual");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEquivalentWithMessage()
        {
            var testFolder = "AreNotEquivalent";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task AreNotEquivalentWithoutMessage()
        {
            var testFolder = "AreNotEquivalent";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 52).WithArguments("AreNotEquivalent");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 52).WithArguments("AreNotEquivalent");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task ContainsWithMessage()
        {
            var testFolder = "Contains";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task ContainsWithoutMessage()
        {
            var testFolder = "Contains";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 44).WithArguments("Contains");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 44).WithArguments("Contains");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task DoesNotContainWithMessage()
        {
            var testFolder = "DoesNotContain";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task DoesNotContainWithoutMessage()
        {
            var testFolder = "DoesNotContain";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 50).WithArguments("DoesNotContain");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 50).WithArguments("DoesNotContain");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task IsNotSubsetOfWithMessage()
        {
            var testFolder = "IsNotSubsetOf";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task IsNotSubsetOfWithoutMessage()
        {
            var testFolder = "IsNotSubsetOf";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 49).WithArguments("IsNotSubsetOf");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 49).WithArguments("IsNotSubsetOf");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task IsSubsetOfWithMessage()
        {
            var testFolder = "IsSubsetOf";
            var testFile = @"MessageBoth.cs";
            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }

        [TestMethod]
        public async Task IsSubsetOfWithoutMessage()
        {
            var testFolder = "IsSubsetOf";
            var testFile = @"NoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 46).WithArguments("IsSubsetOf");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 46).WithArguments("IsSubsetOf");

            var test = new VerifyCS.Test
{
    TestCode = testReader.ReadTest(testFolder, testFile),
    ExpectedDiagnostics = { expected1st, expected2nd },
    ReferenceAssemblies = UnitTestingAssembly
};
test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
await test.RunAsync();
        }



    }
}
