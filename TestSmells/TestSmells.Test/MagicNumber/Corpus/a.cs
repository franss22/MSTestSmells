[TestMethod]
        public async Task NuintNuint()
        {
            var testFile = @"NuintNuint.cs";
            await new VerifyCS.Test
            {
                TestCode = testReader.ReadTest(testFolder, testFile),
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
                TestCode = testReader.ReadTest(testFolder, testFile),
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
                TestCode = testReader.ReadTest(testFolder, testFile),
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
                TestCode = testReader.ReadTest(testFolder, testFile),
                ExpectedDiagnostics = { expected1st, expected2nd },
                ReferenceAssemblies = UnitTestingAssembly
            }.RunAsync();
        }