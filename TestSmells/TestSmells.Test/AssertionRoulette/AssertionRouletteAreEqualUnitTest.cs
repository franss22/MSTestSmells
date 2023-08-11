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

namespace TestSmells.Test.AssertionRoulette
{
    [TestClass]
    public class AssertionRouletteAreEqualUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = NetFramework.Net48.Default.AddPackages(ImmutableArray.Create(new PackageIdentity("Microsoft.VisualStudio.UnitTesting", "11.0.50727.1"))).AddAssemblies(ImmutableArray.Create("Microsoft.VisualStudio.UnitTesting"));

        private readonly TestReader testReader = new TestReader("AssertionRoulette", "Corpus", "AreEqual");
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }


        [TestMethod]
        public async Task SingleAssertIntMessage()
        {
            var testFile = @"SingleAssertIntMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertInt()
        {
            var testFile = @"SingleAssertInt.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertIntNoMessageFirst()
        {
            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 34).WithArguments("AreEqual");

            var testFile = @"DoubleAssertIntNoMessageFirst.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertIntNoMessageSecond()
        {
            var testFile = @"DoubleAssertIntNoMessageSecond.cs";

            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 34).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertIntNoMessageBoth()
        {
            var testFile = @"DoubleAssertIntNoMessageBoth.cs";

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
        public async Task DoubleAssertIntMessage()
        {
            var testFile = @"DoubleAssertIntMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertObjectMessage()
        {
            var testFile = @"SingleAssertObjectMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertObject()
        {
            var testFile = @"SingleAssertObject.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertObjectNoMessageFirst()
        {
            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 34).WithArguments("AreEqual");

            var testFile = @"DoubleAssertObjectNoMessageFirst.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertObjectNoMessageSecond()
        {
            var testFile = @"DoubleAssertObjectNoMessageSecond.cs";

            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 34).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertObjectNoMessageBoth()
        {
            var testFile = @"DoubleAssertObjectNoMessageBoth.cs";

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
        public async Task DoubleAssertObjectMessage()
        {
            var testFile = @"DoubleAssertObjectMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertDoubleMessage()
        {
            var testFile = @"SingleAssertDoubleMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertDouble()
        {
            var testFile = @"SingleAssertDouble.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertDoubleNoMessageFirst()
        {
            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 39).WithArguments("AreEqual");

            var testFile = @"DoubleAssertDoubleNoMessageFirst.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertDoubleNoMessageSecond()
        {
            var testFile = @"DoubleAssertDoubleNoMessageSecond.cs";

            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 39).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertDoubleNoMessageBoth()
        {
            var testFile = @"DoubleAssertDoubleNoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 39).WithArguments("AreEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 39).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertDoubleMessage()
        {
            var testFile = @"DoubleAssertDoubleMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertFloatMessage()
        {
            var testFile = @"SingleAssertFloatMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertFloat()
        {
            var testFile = @"SingleAssertFloat.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertFloatNoMessageFirst()
        {
            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 39).WithArguments("AreEqual");

            var testFile = @"DoubleAssertFloatNoMessageFirst.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertFloatNoMessageSecond()
        {
            var testFile = @"DoubleAssertFloatNoMessageSecond.cs";

            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 39).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertFloatNoMessageBoth()
        {
            var testFile = @"DoubleAssertFloatNoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 39).WithArguments("AreEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 39).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertFloatMessage()
        {
            var testFile = @"DoubleAssertFloatMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertStringMessage()
        {
            var testFile = @"SingleAssertStringMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertString()
        {
            var testFile = @"SingleAssertString.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertStringNoMessageFirst()
        {
            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 41).WithArguments("AreEqual");

            var testFile = @"DoubleAssertStringNoMessageFirst.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertStringNoMessageSecond()
        {
            var testFile = @"DoubleAssertStringNoMessageSecond.cs";

            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 41).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertStringNoMessageBoth()
        {
            var testFile = @"DoubleAssertStringNoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(13, 13, 13, 41).WithArguments("AreEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(17, 13, 17, 41).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertStringMessage()
        {
            var testFile = @"DoubleAssertStringMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertStringCultureInfoMessage()
        {
            var testFile = @"SingleAssertStringCultureInfoMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task SingleAssertStringCultureInfo()
        {
            var testFile = @"SingleAssertStringCultureInfo.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertStringCultureInfoNoMessageFirst()
        {
            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 51).WithArguments("AreEqual");

            var testFile = @"DoubleAssertStringCultureInfoNoMessageFirst.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertStringCultureInfoNoMessageSecond()
        {
            var testFile = @"DoubleAssertStringCultureInfoNoMessageSecond.cs";

            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(19, 13, 19, 51).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertStringCultureInfoNoMessageBoth()
        {
            var testFile = @"DoubleAssertStringCultureInfoNoMessageBoth.cs";

            var expected1st = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(15, 13, 15, 51).WithArguments("AreEqual");
            var expected2nd = VerifyCS.Diagnostic("AssertionRoulette").WithSpan(19, 13, 19, 51).WithArguments("AreEqual");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task DoubleAssertStringCultureInfoMessage()
        {
            var testFile = @"DoubleAssertStringCultureInfoMessage.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


    }
}
