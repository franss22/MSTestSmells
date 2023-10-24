
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Threading.Tasks;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.Compendium.AnalyzerCompendium>;

//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.MagicNumber.MagicNumberAnalyzer,
//    TestSmells.MagicNumber.MagicNumberCodeFixProvider>;
using TestReading;

namespace TestSmells.Test.MagicNumber
{
    [TestClass]
    public class MagicNumberAreNotEqualUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("MagicNumber", "Corpus", "AreNotEqual");

        private readonly (string filename, string content) ExcludeOtherCompendiumDiagnostics = TestOptions.EnableSingleDiagnosticForCompendium("MagicNumber");

        /* 
         * Tests are named by the type of value given to the assertion method
         * L means the value is a literal
         * For example, IntLInt means the assertion method is given an integer literal value, 
         * then an integer value (in a variable)
         * 
         * Types that don't have a literal suffix are cast inside the method (Ex: Assert.AreNotEqual((byte)1, b)
         */



        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyProgram()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ObjObj()
        {
            var testFile = @"ObjObj.cs";
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
        public async Task StringString()
        {
            var testFile = @"StrStr.cs";
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
        public async Task StringStringL()
        {

            var testFile = @"StrStrL.cs";
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
        public async Task StringLString()
        {

            var testFile = @"StrLStr.cs";

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
        public async Task StringLStringL()
        {

            var testFile = @"StrLStrL.cs";

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
        public async Task DoubleDouble()
        {
            var testFile = @"DoubleDouble.cs";
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
        public async Task DoubleDoubleL()
        {

            var testFile = @"DoubleDoubleL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 39).WithArguments("AreNotEqual", "2.5d");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task DoubleLDouble()
        {

            var testFile = @"DoubleLDouble.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 36).WithArguments("AreNotEqual", "1.5d");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task DoubleLDoubleL()
        {

            var testFile = @"DoubleLDoubleL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 36).WithArguments("AreNotEqual", "1.5d");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 38, 12, 42).WithArguments("AreNotEqual", "2.5d");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }




        [TestMethod]
        public async Task FloatFloat()
        {
            var testFile = @"FloatFloat.cs";
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
        public async Task FloatFloatL()
        {

            var testFile = @"FloatFloatL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 39).WithArguments("AreNotEqual", "2.5F");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task FloatLFloat()
        {

            var testFile = @"FloatLFloat.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 36).WithArguments("AreNotEqual", "1.5F");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task FloatLFloatL()
        {

            var testFile = @"FloatLFloatL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 36).WithArguments("AreNotEqual", "1.5F");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 38, 12, 42).WithArguments("AreNotEqual", "2.5F");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }



        [TestMethod]
        public async Task SbyteSbyte()
        {
            var testFile = @"SbyteSbyte.cs";
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
        public async Task SbyteSbyteL()
        {

            var testFile = @"SbyteSbyteL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 43).WithArguments("AreNotEqual", "(sbyte)2");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task SbyteLSbyte()
        {

            var testFile = @"SbyteLSbyte.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 40).WithArguments("AreNotEqual", "(sbyte)1");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task SbyteLSbyteL()
        {

            var testFile = @"SbyteLSbyteL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 40).WithArguments("AreNotEqual", "(sbyte)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 42, 12, 50).WithArguments("AreNotEqual", "(sbyte)2");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task ByteByte()
        {
            var testFile = @"ByteByte.cs";
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
        public async Task ByteByteL()
        {

            var testFile = @"ByteByteL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 42).WithArguments("AreNotEqual", "(byte)2");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task ByteLByte()
        {

            var testFile = @"ByteLByte.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 39).WithArguments("AreNotEqual", "(byte)1");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task ByteLByteL()
        {

            var testFile = @"ByteLByteL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 39).WithArguments("AreNotEqual", "(byte)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 41, 12, 48).WithArguments("AreNotEqual", "(byte)2");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }


        [TestMethod]
        public async Task ShortShort()
        {
            var testFile = @"ShortShort.cs";
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
        public async Task ShortShortL()
        {

            var testFile = @"ShortShortL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 43).WithArguments("AreNotEqual", "(short)2");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task ShortLShort()
        {

            var testFile = @"ShortLShort.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 40).WithArguments("AreNotEqual", "(short)1");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task ShortLShortL()
        {

            var testFile = @"ShortLShortL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 40).WithArguments("AreNotEqual", "(short)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 42, 12, 50).WithArguments("AreNotEqual", "(short)2");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task UshortUshort()
        {
            var testFile = @"UshortUshort.cs";
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
        public async Task UshortUshortL()
        {

            var testFile = @"UshortUshortL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 44).WithArguments("AreNotEqual", "(ushort)2");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task UshortLUshort()
        {

            var testFile = @"UshortLUshort.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 41).WithArguments("AreNotEqual", "(ushort)1");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task UshortLUshortL()
        {

            var testFile = @"UshortLUshortL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 41).WithArguments("AreNotEqual", "(ushort)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 43, 12, 52).WithArguments("AreNotEqual", "(ushort)2");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }


        [TestMethod]
        public async Task IntInt()
        {
            var testFile = @"IntInt.cs";
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
        public async Task IntIntL()
        {

            var testFile = @"IntIntL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 36).WithArguments("AreNotEqual", "2");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task IntLInt()
        {

            var testFile = @"IntLInt.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 33).WithArguments("AreNotEqual", "1");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task IntLIntL()
        {

            var testFile = @"IntLIntL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 33).WithArguments("AreNotEqual", "1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 36).WithArguments("AreNotEqual", "2");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }


        [TestMethod]
        public async Task UintUint()
        {
            var testFile = @"UintUint.cs";
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
        public async Task UintUintL()
        {

            var testFile = @"UintUintL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 37).WithArguments("AreNotEqual", "2u");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task UintLUint()
        {

            var testFile = @"UintLUint.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 34).WithArguments("AreNotEqual", "1u");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task UintLUintL()
        {

            var testFile = @"UintLUintL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 34).WithArguments("AreNotEqual", "1u");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 36, 12, 38).WithArguments("AreNotEqual", "2u");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }


        [TestMethod]
        public async Task LongLong()
        {
            var testFile = @"LongLong.cs";
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
        public async Task LongLongL()
        {

            var testFile = @"LongLongL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 37).WithArguments("AreNotEqual", "2l");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task LongLLong()
        {

            var testFile = @"LongLLong.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 34).WithArguments("AreNotEqual", "1l");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task LongLLongL()
        {

            var testFile = @"LongLLongL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 34).WithArguments("AreNotEqual", "1l");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 36, 12, 38).WithArguments("AreNotEqual", "2l");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }


        [TestMethod]
        public async Task UlongUlong()
        {
            var testFile = @"UlongUlong.cs";
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
        public async Task UlongUlongL()
        {

            var testFile = @"UlongUlongL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 38).WithArguments("AreNotEqual", "2ul");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task UlongLUlong()
        {

            var testFile = @"UlongLUlong.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 35).WithArguments("AreNotEqual", "1ul");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task UlongLUlongL()
        {

            var testFile = @"UlongLUlongL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 35).WithArguments("AreNotEqual", "1ul");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 37, 12, 40).WithArguments("AreNotEqual", "2ul");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task NintNint()
        {
            var testFile = @"NintNint.cs";
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
        public async Task NintNintL()
        {

            var testFile = @"NintNintL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 42).WithArguments("AreNotEqual", "(nint)2");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task NintLNint()
        {

            var testFile = @"NintLNint.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 39).WithArguments("AreNotEqual", "(nint)1");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task NintLNintL()
        {

            var testFile = @"NintLNintL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 39).WithArguments("AreNotEqual", "(nint)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 41, 12, 48).WithArguments("AreNotEqual", "(nint)2");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }

        [TestMethod]
        public async Task NuintNuint()
        {
            var testFile = @"NuintNuint.cs";
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
        public async Task NuintNuintL()
        {

            var testFile = @"NuintNuintL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 43).WithArguments("AreNotEqual", "(nuint)2");
            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task NuintLNuint()
        {

            var testFile = @"NuintLNuint.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 40).WithArguments("AreNotEqual", "(nuint)1");


            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
        [TestMethod]
        public async Task NuintLNuintL()
        {

            var testFile = @"NuintLNuintL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 40).WithArguments("AreNotEqual", "(nuint)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 42, 12, 50).WithArguments("AreNotEqual", "(nuint)2");

            var test = new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            };
            test.TestState.AnalyzerConfigFiles.Add(ExcludeOtherCompendiumDiagnostics);
            await test.RunAsync();
        }
    }
}
