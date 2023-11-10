
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.Compendium.AnalyzerCompendium>;
//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.MagicNumber.MagicNumberAnalyzer,
//    TestSmells.MagicNumber.MagicNumberCodeFixProvider>;
using System.Collections.Immutable;
using TestReading;

namespace TestSmells.Test.MysteryGuest
{
    [TestClass]
    public class MysteryGuestFileReadUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();


        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("MysteryGuest");


        private readonly TestReader testReader = new TestReader("MysteryGuest", "Corpus", "FileRead");

        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ReadAllBytes()
        {
            var testFile = @"ReadAllBytes.cs";

            var diagnostic = VerifyCS.Diagnostic("MysteryGuest").WithSpan(13, 24, 13, 47).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReadAllBytesAsync()
        {
            var testFile = @"ReadAllBytesAsync.cs";

            var diagnostic = VerifyCS.Diagnostic("MysteryGuest").WithSpan(13, 30, 13, 58).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReadAllLines()
        {
            var testFile = @"ReadAllLines.cs";

            var diagnostic = VerifyCS.Diagnostic("MysteryGuest").WithSpan(13, 24, 13, 47).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReadAllLinesAsync()
        {
            var testFile = @"ReadAllLinesAsync.cs";

            var diagnostic = VerifyCS.Diagnostic("MysteryGuest").WithSpan(13, 30, 13, 58).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReadAllText()
        {
            var testFile = @"ReadAllText.cs";

            var diagnostic = VerifyCS.Diagnostic("MysteryGuest").WithSpan(13, 24, 13, 46).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReadAllTextAsync()
        {
            var testFile = @"ReadAllTextAsync.cs";

            var diagnostic = VerifyCS.Diagnostic("MysteryGuest").WithSpan(13, 30, 13, 57).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReadLines()
        {
            var testFile = @"ReadLines.cs";

            var diagnostic = VerifyCS.Diagnostic("MysteryGuest").WithSpan(13, 24, 13, 44).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReadLinesAsync()
        {
            var testFile = @"ReadLinesAsync.cs";

            var diagnostic = VerifyCS.Diagnostic("MysteryGuest").WithSpan(13, 41, 13, 66).WithArguments("TestMethod");

            //ReadLinesAsync only exists in .Net 7 and 8
            //But the reference is not working right now

            var net7Assemblies = Net.Net70
                .AddPackages(ImmutableArray.Create(new PackageIdentity("MSTest.TestFramework", "3.1.1")))
                .AddAssemblies(ImmutableArray.Create("Microsoft.VisualStudio.UnitTesting"));

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = net7Assemblies
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task OpenRead()
        {
            var testFile = @"OpenRead.cs";

            var diagnostic = VerifyCS.Diagnostic("MysteryGuest").WithSpan(13, 24, 13, 43).WithArguments("TestMethod");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { diagnostic },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

 

    }
}
