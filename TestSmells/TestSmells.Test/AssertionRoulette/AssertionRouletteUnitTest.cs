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

namespace TestSmells.Test.AssertionRoulette
{
    [TestClass]
    public class AssertionRouletteUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = NetFramework.Net48.Default.AddPackages(ImmutableArray.Create(new PackageIdentity("Microsoft.VisualStudio.UnitTesting", "11.0.50727.1"))).AddAssemblies(ImmutableArray.Create("Microsoft.VisualStudio.UnitTesting"));

        private readonly TestReader testReader = new TestReader("AssertionRoulette", "Corpus");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }


        [TestMethod]
        public async Task SingleAssertIntIntMessage()
        {
            var testFile = @"SingleAssertIntIntMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertIntInt()
        {
            var testFile = @"SingleAssertIntInt.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertIntIntNoMessageFirst()
        {
            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 34).WithArguments("AreEqual");

            var testFile = @"DoubleAssertIntIntNoMessageFirst.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertIntIntNoMessageSecond()
        {
            var testFile = @"DoubleAssertIntIntNoMessageSecond.cs";

            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 34).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertIntIntNoMessageBoth()
        {
            var testFile = @"DoubleAssertIntIntNoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 34).WithArguments("AreEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 34).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertIntIntMessage()
        {
            var testFile = @"DoubleAssertIntIntMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }



    }
}
