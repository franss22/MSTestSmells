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

        public string ReadTest(string filePath, params string[] filePaths)
        {
            var path = Path.Combine(basePath, filePath);
            foreach (var pathPart in filePaths)
            {
                path = Path.Combine(path, pathPart);
            }
            return File.ReadAllText(path);
        }
    }
}
