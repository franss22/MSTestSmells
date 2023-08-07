using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TestReading
{
    public class TestReader
    {
        string basePath;

        public TestReader(string testClassFolder)
        {
            string workingDirectory = Environment.CurrentDirectory;
            basePath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            basePath = Path.Combine(basePath, testClassFolder);
        }

        public string ReadTest(string fileName)
        {
            return File.ReadAllText(Path.Combine(basePath, fileName));
        }
    }
}
