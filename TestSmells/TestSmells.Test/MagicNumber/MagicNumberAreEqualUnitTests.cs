
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;
using VerifyCS = TestSmells.Test.CSharpAnalyzerVerifier<TestSmells.MagicNumber.MagicNumberAnalyzer>;
//using VerifyCS = TestSmells.Test.CSharpCodeFixVerifier<
//    TestSmells.MagicNumber.MagicNumberAnalyzer,
//    TestSmells.MagicNumber.MagicNumberCodeFixProvider>;
using System.Collections.Immutable;
using TestReading;
using System;

namespace TestSmells.Test.MagicNumber
{
    [TestClass]
    public class MagicNumberAreEqualUnitTests

    {

        private readonly ReferenceAssemblies UnitTestingAssembly = TestSmellReferenceAssembly.Assemblies();

        private readonly TestReader testReader = new TestReader("MagicNumber", "Corpus", "AreEqual");
        /* 
         * Tests are named by the type of value given to the assertion method
         * L means the value is a literal
         * For example, IntLInt means the assertion method is given an integer literal value, 
         * then an integer value (in a variable)
         * 
         * Types that don't have a literal suffix are cast inside the method (Ex: Assert.AreEqual((byte)1, b)
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
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task StringString()
        {
            var testFile = @"StrStr.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task StringStringL()
        {

            var testFile = @"StrStrL.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task StringLString()
        {

            var testFile = @"StrLStr.cs";

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task StringLStringL()
        {

            var testFile = @"StrLStrL.cs";

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task DoubleDouble()
        {
            var testFile = @"DoubleDouble.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task DoubleDoubleL()
        {

            var testFile = @"DoubleDoubleL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 36).WithArguments("AreEqual", "2.5d");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task DoubleLDouble()
        {

            var testFile = @"DoubleLDouble.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 33).WithArguments("AreEqual", "1.5d");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task DoubleLDoubleL()
        {

            var testFile = @"DoubleLDoubleL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 33).WithArguments("AreEqual", "1.5d");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 39).WithArguments("AreEqual", "2.5d");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }




        [TestMethod]
        public async Task FloatFloat()
        {
            var testFile = @"FloatFloat.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task FloatFloatL()
        {

            var testFile = @"FloatFloatL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 36).WithArguments("AreEqual", "2.5F");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task FloatLFloat()
        {

            var testFile = @"FloatLFloat.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 33).WithArguments("AreEqual", "1.5F");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task FloatLFloatL()
        {

            var testFile = @"FloatLFloatL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 33).WithArguments("AreEqual", "1.5F");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 35, 12, 39).WithArguments("AreEqual", "2.5F");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }



        [TestMethod]
        public async Task SbyteSbyte()
        {
            var testFile = @"SbyteSbyte.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task SbyteSbyteL()
        {

            var testFile = @"SbyteSbyteL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 40).WithArguments("AreEqual", "(sbyte)2");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task SbyteLSbyte()
        {

            var testFile = @"SbyteLSbyte.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 37).WithArguments("AreEqual", "(sbyte)1");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task SbyteLSbyteL()
        {

            var testFile = @"SbyteLSbyteL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 37).WithArguments("AreEqual", "(sbyte)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 39, 12, 47).WithArguments("AreEqual", "(sbyte)2");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ByteByte()
        {
            var testFile = @"ByteByte.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task ByteByteL()
        {

            var testFile = @"ByteByteL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 39).WithArguments("AreEqual", "(byte)2");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task ByteLByte()
        {

            var testFile = @"ByteLByte.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 36).WithArguments("AreEqual", "(byte)1");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task ByteLByteL()
        {

            var testFile = @"ByteLByteL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 36).WithArguments("AreEqual", "(byte)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 38, 12, 45).WithArguments("AreEqual", "(byte)2");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task ShortShort()
        {
            var testFile = @"ShortShort.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task ShortShortL()
        {

            var testFile = @"ShortShortL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 40).WithArguments("AreEqual", "(short)2");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task ShortLShort()
        {

            var testFile = @"ShortLShort.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 37).WithArguments("AreEqual", "(short)1");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task ShortLShortL()
        {

            var testFile = @"ShortLShortL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 37).WithArguments("AreEqual", "(short)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 39, 12, 47).WithArguments("AreEqual", "(short)2");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task UshortUshort()
        {
            var testFile = @"UshortUshort.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task UshortUshortL()
        {

            var testFile = @"UshortUshortL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 41).WithArguments("AreEqual", "(ushort)2");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task UshortLUshort()
        {

            var testFile = @"UshortLUshort.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 38).WithArguments("AreEqual", "(ushort)1");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task UshortLUshortL()
        {

            var testFile = @"UshortLUshortL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 38).WithArguments("AreEqual", "(ushort)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 40, 12, 49).WithArguments("AreEqual", "(ushort)2");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task IntInt()
        {
            var testFile = @"IntInt.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task IntIntL()
        {

            var testFile = @"IntIntL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 33).WithArguments("AreEqual", "2");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task IntLInt()
        {

            var testFile = @"IntLInt.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 30).WithArguments("AreEqual", "1");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task IntLIntL()
        {

            var testFile = @"IntLIntL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 30).WithArguments("AreEqual", "1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 33).WithArguments("AreEqual", "2");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task UintUint()
        {
            var testFile = @"UintUint.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task UintUintL()
        {

            var testFile = @"UintUintL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 34).WithArguments("AreEqual", "2u");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task UintLUint()
        {

            var testFile = @"UintLUint.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 31).WithArguments("AreEqual", "1u");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task UintLUintL()
        {

            var testFile = @"UintLUintL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 31).WithArguments("AreEqual", "1u");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 33, 12, 35).WithArguments("AreEqual", "2u");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task LongLong()
        {
            var testFile = @"LongLong.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task LongLongL()
        {

            var testFile = @"LongLongL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 34).WithArguments("AreEqual", "2l");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task LongLLong()
        {

            var testFile = @"LongLLong.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 31).WithArguments("AreEqual", "1l");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task LongLLongL()
        {

            var testFile = @"LongLLongL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 31).WithArguments("AreEqual", "1l");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 33, 12, 35).WithArguments("AreEqual", "2l");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task UlongUlong()
        {
            var testFile = @"UlongUlong.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task UlongUlongL()
        {

            var testFile = @"UlongUlongL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 35).WithArguments("AreEqual", "2ul");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task UlongLUlong()
        {

            var testFile = @"UlongLUlong.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 32).WithArguments("AreEqual", "1ul");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task UlongLUlongL()
        {

            var testFile = @"UlongLUlongL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 32).WithArguments("AreEqual", "1ul");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 34, 12, 37).WithArguments("AreEqual", "2ul");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task NintNint()
        {
            var testFile = @"NintNint.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task NintNintL()
        {

            var testFile = @"NintNintL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 39).WithArguments("AreEqual", "(nint)2");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task NintLNint()
        {

            var testFile = @"NintLNint.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 36).WithArguments("AreEqual", "(nint)1");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task NintLNintL()
        {

            var testFile = @"NintLNintL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 36).WithArguments("AreEqual", "(nint)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 38, 12, 45).WithArguments("AreEqual", "(nint)2");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }

        [TestMethod]
        public async Task NuintNuint()
        {
            var testFile = @"NuintNuint.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }


        [TestMethod]
        public async Task NuintNuintL()
        {

            var testFile = @"NuintNuintL.cs";
            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 32, 12, 40).WithArguments("AreEqual", "(nuint)2");
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task NuintLNuint()
        {

            var testFile = @"NuintLNuint.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 37).WithArguments("AreEqual", "(nuint)1");


            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
        [TestMethod]
        public async Task NuintLNuintL()
        {

            var testFile = @"NuintLNuintL.cs";
            var expected1st = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 29, 12, 37).WithArguments("AreEqual", "(nuint)1");

            var expected2nd = VerifyCS.Diagnostic("MagicNumber").WithSpan(12, 39, 12, 47).WithArguments("AreEqual", "(nuint)2");

            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }
    }
}
